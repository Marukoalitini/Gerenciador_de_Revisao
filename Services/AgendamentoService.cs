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

    public async Task<RevisaoSemClienteEMotoResponse?> ObterProximaRevisaoAsync(int clienteId, string placa)
    {
        var revisao = await _db.Revisoes
            .Include(r => r.Itens)
            .Include(r => r.ConcessionariaResponsavel)
            .ThenInclude(c => c.Enderecos)
            .Where(r => r.ClienteId == clienteId && r.Moto.Placa == placa && (r.Status == StatusRevisao.Pendente || r.Status == StatusRevisao.AguardandoConfirmacao || r.Status == StatusRevisao.Cancelada ) )
            .OrderBy(r => r.Numero)
            .FirstOrDefaultAsync();

        return revisao == null ? null : _mapper.Map<RevisaoSemClienteEMotoResponse>(revisao);
    }

    public async Task<RevisaoSemClienteEMotoResponse> SolicitarAgendamentoAsync(string placa, int clienteId, SolicitarAgendamentoRequest request)
    {
        var (motoEncontrada, revisao) = await ObterMotoERevisaoParaAgendamentoAsync(placa, clienteId);

        var concessionaria = await _db.Concessionarias.FindAsync(request.ConcessionariaId);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        ValidarDataAgendamento(motoEncontrada, revisao, request.DataDesejada);

        revisao.DataAgendada = request.DataDesejada;
        revisao.ConcessionariaResponsavelId = request.ConcessionariaId;
        revisao.Status = StatusRevisao.AguardandoConfirmacao;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        return _mapper.Map<RevisaoSemClienteEMotoResponse>(revisao);
    }

    public async Task<List<RevisaoSemConcessionariaResponse>> ListarSolicitacoesAsync(int concessionariaId)
    {
        var revisoes = await _db.Revisoes
            .Include(r => r.Moto)
            .Include(r => r.Cliente)
            .Where(r => r.ConcessionariaResponsavelId == concessionariaId && r.Status == StatusRevisao.AguardandoConfirmacao)
            .OrderBy(r => r.DataAgendada)
            .ToListAsync();

        return _mapper.Map<List<RevisaoSemConcessionariaResponse>>(revisoes);
    }

    public async Task<RevisaoSemConcessionariaResponse> ConfirmarAgendamentoAsync(int revisaoId, int concessionariaId)
    {
        var revisao = await _db.Revisoes.FindAsync(revisaoId);
        if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

        if (revisao.ConcessionariaResponsavelId != concessionariaId)
            throw new BadRequestException("Revisão não pertence a esta concessionária.");

        if (revisao.Status != StatusRevisao.AguardandoConfirmacao)
            throw new BadRequestException($"Não é possível confirmar uma revisão com status {revisao.Status}.");

        revisao.Status = StatusRevisao.Agendada;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        // enviar notificação por e-mail ao cliente
        var cliente = await _db.Clientes.FindAsync(revisao.ClienteId);
        if (cliente != null)
        {
            await _notificacaoService.EnviarNotificacaoStatusAgendamentoAsync(cliente.Email, revisao.Moto?.Placa ?? "--", revisao.Numero, revisao.DataAgendada, true);
        }

        return _mapper.Map<RevisaoSemConcessionariaResponse>(revisao);
    }

    public async Task<RevisaoSemConcessionariaResponse> RecusarAgendamentoAsync(int revisaoId, int concessionariaId)
    {
        var revisao = await _db.Revisoes.FindAsync(revisaoId);
        if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

        if (revisao.ConcessionariaResponsavelId != concessionariaId)
            throw new BadRequestException("Revisão não pertence a esta concessionária.");

        if (revisao.Status != StatusRevisao.AguardandoConfirmacao)
            throw new BadRequestException($"Não é possível recusar uma revisão com status {revisao.Status}.");

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

        return _mapper.Map<RevisaoSemConcessionariaResponse>(revisao);
    }

    public async Task<RevisaoSemClienteEMotoResponse> ReagendarAgendamentoAsync(string placa, int clienteId, AgendarRevisaoRequest request)
    {
        var (moto, revisao) = await ObterMotoERevisaoParaAgendamentoAsync(placa, clienteId, true);

        var concessionaria = await _db.Concessionarias.FindAsync(request.ConcessionariaId);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        ValidarDataAgendamento(moto, revisao, request.DataAgendamento);

        revisao.DataAgendada = request.DataAgendamento;
        revisao.ConcessionariaResponsavelId = request.ConcessionariaId;
        revisao.Status = StatusRevisao.AguardandoConfirmacao;

        _db.Revisoes.Update(revisao);
        await _db.SaveChangesAsync();

        return _mapper.Map<RevisaoSemClienteEMotoResponse>(revisao);
    }

    private async Task<(Moto Moto, Revisao Revisao)> ObterMotoERevisaoParaAgendamentoAsync(string placa, int clienteId, bool isReagendamento = false)
    {
        var moto = await _db.Motos.Include(m => m.Revisoes).FirstOrDefaultAsync(m => m.Placa == placa);
        if (moto == null) throw new NotFoundException("Moto não encontrada.");

        if (moto.ClienteId != clienteId)
            throw new BadRequestException("Moto não pertence ao cliente.");

        var statusPermitidos = new[] { StatusRevisao.Pendente, StatusRevisao.AguardandoConfirmacao, StatusRevisao.Cancelada };
        if (isReagendamento)
        {
            statusPermitidos = [StatusRevisao.Agendada, StatusRevisao.AguardandoConfirmacao, StatusRevisao.Cancelada];
        }

        var revisao = await _db.Revisoes
            .Include(r => r.Itens)
            .Include(r => r.ConcessionariaResponsavel)
            .ThenInclude(c => c.Enderecos)
            .Where(r => r.Moto.Placa == placa && statusPermitidos.Contains(r.Status))
            .OrderBy(r => r.Numero)
            .FirstOrDefaultAsync();

        if (revisao == null)
            throw new NotFoundException(isReagendamento ? "Nenhuma revisão disponível para reagendamento encontrada." : "Nenhuma revisão pendente encontrada para esta moto.");

        return (moto, revisao);
    }

    private void ValidarDataAgendamento(Moto moto, Revisao revisao, DateOnly dataDesejada)
    {
        var infoPrevisao = CalcularPrevisao(moto);
        if (infoPrevisao != null)
        {
            var minimo = infoPrevisao.DataPrevista.AddDays(-15);
            var maximo = infoPrevisao.DataPrevista.AddDays(15);
            if (dataDesejada < minimo || dataDesejada > maximo)
                throw new BadRequestException($"Data de agendamento inválida. A revisão {revisao.Numero} só pode ser agendada entre {minimo:yyyy-MM-dd} e {maximo:yyyy-MM-dd} (data prevista: {infoPrevisao.DataPrevista:yyyy-MM-dd}).");
        }
    }


    public InfoPrevisaoRevisao? CalcularPrevisao(Moto moto)
    {
        var ultimaRevisao = moto.Revisoes
            .Where(r => r.Status == StatusRevisao.Executada)
            .OrderByDescending(r => r.DataExecucao ?? DateOnly.MinValue)
            .ThenByDescending(r => r.Numero)
            .FirstOrDefault();

        int proximaRevisaoNum;
        DateOnly dataBase;
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

        DateOnly dataPrevista = dataBase.AddMonths(mesesParaAdicionar);
        var diasRestantes = (dataPrevista.ToDateTime(TimeOnly.MinValue) - DateTime.Now.Date).TotalDays;

        return new InfoPrevisaoRevisao(
            proximaRevisaoNum,
            dataPrevista,
            diasRestantes < 0,
            (int)diasRestantes
        );
    }
}
