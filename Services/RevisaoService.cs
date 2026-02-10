using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Exceptions;
using Motos.Models;

namespace Motos.Services;

public class RevisaoService
{
	private readonly AppDbContext _db;
	private readonly ChecklistService _checklistService;

	public RevisaoService(AppDbContext db, ChecklistService checklistService)
	{
		_db = db;
		_checklistService = checklistService;
	}

	public async Task<Revisao> CriarRevisaoAsync(RevisaoRequest req)
	{
		// Valida Moto
		var moto = await _db.Motos.FindAsync(req.MotoId);
		if (moto == null) throw new DomainException("Moto não encontrada.");

		// Valida Cliente
		var cliente = await _db.Clientes.FindAsync(req.ClienteId);
		if (cliente == null) throw new DomainException("Cliente não encontrado.");

		if (moto.ClienteId.HasValue && moto.ClienteId != req.ClienteId)
			throw new DomainException("Moto não pertence ao cliente informado.");

		// Valida concessionaria (opcional)
		Concessionaria? concessionaria = null;
		if (req.ConcessionariaId.HasValue)
		{
			concessionaria = await _db.Concessionarias.FindAsync(req.ConcessionariaId.Value);
			if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");
		}

		// Monta Revisao
		var revisao = new Revisao
		{
			Numero = req.Numero,
			ClienteId = req.ClienteId,
			MotoId = req.MotoId,
			KmMaximo = req.KmMaximo,
			TempoMaximo = req.TempoMaximo,
			KmAtual = req.KmAtual,
			DataRevisao = req.DataRevisao,
			ValorTotal = req.ValorTotal,
			NotaDeServico = req.NotaDeServico,
			ConcessionariaId = req.ConcessionariaId,
		};

		// Gerar itens via ChecklistService, se possível
		if (moto.Modelo != null)
		{
			var itens = _checklistService.GerarItensParaRevisao(moto.Modelo!.ToString(), revisao.Numero);
			revisao.Itens = itens;
			revisao.ValorTotal = itens.Sum(i => i.Valor ?? 0.0);
		}

		await using var tx = await _db.Database.BeginTransactionAsync();
		try
		{
			_db.Revisoes.Add(revisao);
			await _db.SaveChangesAsync();

			// Ensure moto owner set
			if (!moto.ClienteId.HasValue)
			{
				moto.ClienteId = req.ClienteId;
				_db.Motos.Update(moto);
				await _db.SaveChangesAsync();
			}

			await tx.CommitAsync();
		}
		catch
		{
			await tx.RollbackAsync();
			throw;
		}

		return revisao;
	}

	public async Task<Revisao?> ObterPorIdAsync(int id)
	{
		return await _db.Revisoes
			.Include(r => r.Itens)
			.Include(r => r.Cliente)
			.Include(r => r.Moto)
			.Include(r => r.ConcessinariaResposavel)
			.FirstOrDefaultAsync(r => r.Id == id);
	}

	public async Task<List<Revisao>> ListarAsync(int? concessionariaId = null, int? clienteId = null)
	{
		var query = _db.Revisoes
			.Include(r => r.Itens)
			.Include(r => r.Cliente)
			.Include(r => r.Moto)
			.AsQueryable();

		if (concessionariaId.HasValue) query = query.Where(r => r.ConcessionariaId == concessionariaId.Value);
		if (clienteId.HasValue) query = query.Where(r => r.ClienteId == clienteId.Value);

		return await query.OrderByDescending(r => r.DataRevisao).ToListAsync();
	}

	public async Task ExecutarRevisaoAsync(int id)
	{
		var revisao = await _db.Revisoes.FindAsync(id);
		if (revisao == null) throw new DomainException("Revisão não encontrada.");

		revisao.Status = Enums.StatusRevisao.Executada;
		_db.Revisoes.Update(revisao);
		await _db.SaveChangesAsync();
	}

	public async Task DeletarAsync(int id)
	{
		var revisao = await _db.Revisoes.FindAsync(id);
		if (revisao == null) throw new DomainException("Revisão não encontrada.");

		_db.Revisoes.Remove(revisao);
		await _db.SaveChangesAsync();
	}
}
