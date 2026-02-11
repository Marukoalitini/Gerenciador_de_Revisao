using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Dto.Response;
using Motos.Exceptions;

namespace Motos.Services;

public class RevisaoService
{
	private readonly AppDbContext _db;
	private readonly IMapper _mapper;

	public RevisaoService(AppDbContext db, IMapper mapper)
	{
		_db = db;
		_mapper = mapper;
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
}
