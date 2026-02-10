using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Exceptions;
using Motos.Models;

namespace Motos.Services;

public class RevisaoService
{
	private readonly AppDbContext _db;
	private readonly ChecklistService _checklistService;
	private readonly IMapper _mapper;

	public RevisaoService(AppDbContext db, ChecklistService checklistService, IMapper mapper)
	{
		_db = db;
		_checklistService = checklistService;
		_mapper = mapper;
	}

	public async Task<RevisaoResponse> CriarRevisaoAsync(RevisaoRequest req)
	{
		// Valida Moto
		var moto = await _db.Motos.FindAsync(req.MotoId);
		if (moto == null) throw new NotFoundException("Moto não encontrada.");

		// Valida Cliente
		var cliente = await _db.Clientes.FindAsync(req.ClienteId);
		if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

		if (moto.ClienteId != req.ClienteId)
			throw new DomainException("Moto não pertence ao cliente informado.");

		// Valida concessionaria (opcional)
		if (req.ConcessionariaResponsavelId.HasValue)
		{
			var concessionaria = await _db.Concessionarias.FindAsync(req.ConcessionariaResponsavelId.Value);
			if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");
		}

		// Monta Revisao usando AutoMapper
		var revisao = _mapper.Map<Revisao>(req);

		// Gerar itens via ChecklistService, se possível
		var itens = _checklistService.GerarItensParaRevisao(moto.ModeloMoto.ToString(), revisao.Numero);
		revisao.Itens = itens;
		revisao.ValorTotal = itens.Sum(i => i.Valor ?? 0.0);
		

		await using var tx = await _db.Database.BeginTransactionAsync();
		try
		{
			_db.Revisoes.Add(revisao);
			await _db.SaveChangesAsync();
			
			await tx.CommitAsync();
		}
		catch
		{
			await tx.RollbackAsync();
			throw;
		}

		return _mapper.Map<RevisaoResponse>(revisao);
	}

	public async Task<RevisaoResponse> ObterPorIdAsync(int id)
	{
		var revisao = await _db.Revisoes
			.Include(r => r.Itens)
			.Include(r => r.Cliente)
			.Include(r => r.Moto)
			.Include(r => r.ConcessionariaResponsavel)
			.FirstOrDefaultAsync(r => r.Id == id);

		if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

		return _mapper.Map<RevisaoResponse>(revisao);
	}

	public async Task<List<RevisaoResponse>> ListarAsync(int? concessionariaId = null, int? clienteId = null)
	{
		var query = _db.Revisoes
			.Include(r => r.Itens)
			.Include(r => r.Cliente)
			.Include(r => r.Moto)
			.AsQueryable();

		if (concessionariaId.HasValue) query = query.Where(r => r.ConcessionariaResponsavelId == concessionariaId.Value);
		if (clienteId.HasValue) query = query.Where(r => r.ClienteId == clienteId.Value);

		var list = await query.OrderByDescending(r => r.DataAgendada).ToListAsync();
		return _mapper.Map<List<RevisaoResponse>>(list);
	}

	public async Task ExecutarRevisaoAsync(int id)
	{
		var revisao = await _db.Revisoes.FindAsync(id);
		if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

		revisao.Status = Enums.StatusRevisao.Executada;
		_db.Revisoes.Update(revisao);
		await _db.SaveChangesAsync();
	}

	public async Task DeletarAsync(int id)
	{
		var revisao = await _db.Revisoes.FindAsync(id);
		if (revisao == null) throw new NotFoundException("Revisão não encontrada.");

		_db.Revisoes.Remove(revisao);
		await _db.SaveChangesAsync();
	}
}
