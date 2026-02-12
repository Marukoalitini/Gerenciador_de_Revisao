using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Enums;
using Motos.Exceptions;
using Motos.Models;

namespace Motos.Services;

public class AgendamentoService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly RegraRevisaoService _regraService;
    private readonly INotificacaoService _notificacaoService;

    public AgendamentoService(AppDbContext db, IMapper mapper, RegraRevisaoService regraService, INotificacaoService notificacaoService)
    {
        _db = db;
        _mapper = mapper;
        _regraService = regraService;
        _notificacaoService = notificacaoService;
    }

    public async Task<RevisaoResponse?> ObterProximaRevisaoAsync(int clienteId)
    {
        var revisao = await _db.Revisoes
            .Include(r => r.Moto)
            .Include(r => r.Cliente)
            .Where(r => r.ClienteId == clienteId && r.Status == StatusRevisao.Pendente)
            .OrderBy(r => r.Numero)
            .FirstOrDefaultAsync();

        return revisao == null ? null : _mapper.Map<RevisaoResponse>(revisao);
    }

    public async Task<RevisaoResponse> SolicitarAgendamentoAsync(int revisaoId, int clienteId, SolicitarAgendamentoRequest request)
    {
        var revisao = await _db.Revisoes.FindAsync(revisaoId);
        if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

        if (revisao.ClienteId != clienteId)
            throw new DomainException("Revisão não pertence ao cliente.");

        if (revisao.Status != StatusRevisao.Pendente && revisao.Status != StatusRevisao.AguardandoConfirmacao)
            throw new DomainException($"Não é possível agendar uma revisão com status {revisao.Status}.");

        var concessionaria = await _db.Concessionarias.FindAsync(request.ConcessionariaId);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        revisao.DataAgendada = request.DataDesejada;
        revisao.ConcessionariaResponsavelId = request.ConcessionariaId;
        revisao.Status = StatusRevisao.AguardandoConfirmacao;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        return _mapper.Map<RevisaoResponse>(revisao);
    }

    public async Task<List<RevisaoResponse>> ListarSolicitacoesAsync(int concessionariaId)
    {
        var revisoes = await _db.Revisoes
            .Include(r => r.Moto)
            .Include(r => r.Cliente)
            .Where(r => r.ConcessionariaResponsavelId == concessionariaId && r.Status == StatusRevisao.AguardandoConfirmacao)
            .OrderBy(r => r.DataAgendada)
            .ToListAsync();

        return _mapper.Map<List<RevisaoResponse>>(revisoes);
    }

    public async Task<RevisaoResponse> ConfirmarAgendamentoAsync(int revisaoId, int concessionariaId)
    {
        var revisao = await _db.Revisoes.FindAsync(revisaoId);
        if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

        if (revisao.ConcessionariaResponsavelId != concessionariaId)
            throw new DomainException("Revisão não pertence a esta concessionária.");

        if (revisao.Status != StatusRevisao.AguardandoConfirmacao)
            throw new DomainException($"Não é possível confirmar uma revisão com status {revisao.Status}.");

        revisao.Status = StatusRevisao.Agendada;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        // enviar notificação por e-mail ao cliente
        var cliente = await _db.Clientes.FindAsync(revisao.ClienteId);
        if (cliente != null)
        {
            await _notificacaoService.EnviarNotificacaoStatusAgendamentoAsync(cliente.Email, revisao.Moto?.Placa ?? "--", revisao.Numero, revisao.DataAgendada, true);
        }

        return _mapper.Map<RevisaoResponse>(revisao);
    }

    public async Task<RevisaoResponse> RecusarAgendamentoAsync(int revisaoId, int concessionariaId)
    {
        var revisao = await _db.Revisoes.FindAsync(revisaoId);
        if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

        if (revisao.ConcessionariaResponsavelId != concessionariaId)
            throw new DomainException("Revisão não pertence a esta concessionária.");

        if (revisao.Status != StatusRevisao.AguardandoConfirmacao)
            throw new DomainException($"Não é possível recusar uma revisão com status {revisao.Status}.");

        var dataSolicitada = revisao.DataAgendada;

        revisao.Status = StatusRevisao.Cancelada;
        revisao.DataAgendada = null;
        revisao.ConcessionariaResponsavelId = null;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        // enviar notificação por e-mail ao cliente
        var cliente = await _db.Clientes.FindAsync(revisao.ClienteId);
        if (cliente != null)
        {
            await _notificacaoService.EnviarNotificacaoStatusAgendamentoAsync(cliente.Email, revisao.Moto?.Placa ?? "--", revisao.Numero, dataSolicitada, false);
        }

        return _mapper.Map<RevisaoResponse>(revisao);
    }

    public async Task<RevisaoResponse> ReagendarAgendamentoAsync(int revisaoId, int clienteId, AgendarRevisaoRequest request)
    {
        var revisao = await _db.Revisoes.FindAsync(revisaoId);
        if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

        if (revisao.ClienteId != clienteId)
            throw new DomainException("Revisão não pertence ao cliente.");

        // permitir reagendamento também quando a revisão estiver Cancelada
        if (revisao.Status != StatusRevisao.Agendada && revisao.Status != StatusRevisao.AguardandoConfirmacao && revisao.Status != StatusRevisao.Cancelada)
            throw new DomainException($"Não é possível reagendar uma revisão com status {revisao.Status}.");

        var concessionaria = await _db.Concessionarias.FindAsync(request.ConcessionariaId);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        revisao.DataAgendada = request.DataAgendamento;
        revisao.ConcessionariaResponsavelId = request.ConcessionariaId;
        revisao.Status = StatusRevisao.AguardandoConfirmacao;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        return _mapper.Map<RevisaoResponse>(revisao);
    }

    public InfoPrevisaoRevisao? CalcularPrevisao(Moto moto)
    {
        var ultimaRevisao = moto.Revisoes
            .Where(r => r.Status == StatusRevisao.Executada)
            .OrderByDescending(r => r.DataExecucao ?? DateTime.MinValue)
            .ThenByDescending(r => r.Numero)
            .FirstOrDefault();

        int proximaRevisaoNum;
        DateTime dataBase;
        int mesesParaAdicionar;

        if (ultimaRevisao != null)
        {
            proximaRevisaoNum = ultimaRevisao.Numero + 1;
            var regraUltima = _regraService.ObterPorNumero(ultimaRevisao.Numero);
            var regraProxima = _regraService.ObterPorNumero(proximaRevisaoNum);

            if (regraUltima == null || regraProxima == null || !ultimaRevisao.DataExecucao.HasValue)
                return null;

            dataBase = ultimaRevisao.DataExecucao.Value;
            mesesParaAdicionar = regraProxima.MesesAlvo - regraUltima.MesesAlvo;
        }
        else
        {
            proximaRevisaoNum = 1;
            var regraProxima = _regraService.ObterPorNumero(proximaRevisaoNum);
            
            if (regraProxima == null) return null;

            dataBase = moto.DataDeVenda;
            mesesParaAdicionar = regraProxima.MesesAlvo;
        }

        DateTime dataPrevista = dataBase.AddMonths(mesesParaAdicionar);
        var diasRestantes = (dataPrevista - DateTime.Now).TotalDays;

        return new InfoPrevisaoRevisao(
            proximaRevisaoNum,
            dataPrevista,
            diasRestantes < 0,
            (int)diasRestantes
        );
    }
}
