using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Exceptions;
using Motos.Models;
using Motos.Enums;

namespace Motos.Services;

public class MotoService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ManualRevisoesProvider _revisoesProvider;

    public MotoService(AppDbContext context, IMapper mapper, ManualRevisoesProvider revisoesProvider)
    {
        _context = context;
        _mapper = mapper;
        _revisoesProvider = revisoesProvider;
    }

    public List<ModeloMotoResponse> ObterModelos()
    {
        return Enum.GetValues<ModeloMoto>()
            .Cast<ModeloMoto>()
            .Select(m => new ModeloMotoResponse((int)m, m.ToString(), m.GetDisplayName()))
            .ToList();
    }

    public MotoComRevisoesResponse CadastrarMoto(int clienteId, MotoRequest request)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == clienteId && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        // Verificar se já existe moto com esse chassi ou placa
        if (_context.Motos.Any(m => m.NumeroChassi == request.NumeroChassi || m.Placa == request.Placa))
            throw new ConflictException("Moto com este chassi ou placa já cadastrada.");

        var moto = _mapper.Map<Moto>(request);
        moto.ClienteId = clienteId;
        moto.Cliente = cliente;

        // Gerar todas as revisões previstas para o modelo da moto já vinculando Cliente e Moto
        var revisoes = _revisoesProvider.ObterRevisoesPara(moto.ModeloMoto, cliente, moto);
        moto.Revisoes = revisoes;

        _context.Motos.Add(moto);
        _context.SaveChanges();

        return _mapper.Map<MotoComRevisoesResponse>(moto);
    }

    public List<MotoComRevisoesResponse> ObterMotosDoCliente(int clienteId)
    {
        var motos = _context.Motos
            .Include(m => m.Revisoes)
                .ThenInclude(r => r.Itens)
            .Where(m => m.ClienteId == clienteId && m.Ativo)
            .ToList();

        return _mapper.Map<List<MotoComRevisoesResponse>>(motos);
    }

    public MotoComRevisoesResponse ObterMotoPorId(int clienteId, int motoId)
    {
        var moto = _context.Motos
            .Include(m => m.Revisoes)
                .ThenInclude(r => r.Itens)
            .FirstOrDefault(m => m.Id == motoId && m.ClienteId == clienteId && m.Ativo);
        if (moto == null) throw new NotFoundException("Moto não encontrada ou não pertence ao cliente.");

        return _mapper.Map<MotoComRevisoesResponse>(moto);
    }

    public MotoComRevisoesResponse EditarMoto(int clienteId, string placa, AtualizarMotoRequest request)
    {
        var moto = _context.Motos.FirstOrDefault(m => m.Placa == placa && m.ClienteId == clienteId && m.Ativo);
        if (moto == null) throw new NotFoundException("Moto não encontrada ou não pertence ao cliente.");

        _mapper.Map(request, moto);

        _context.SaveChanges();

        return _mapper.Map<MotoComRevisoesResponse>(moto);
    }

    public void RemoverMoto(int clienteId, string placa)
    {
        var moto = _context.Motos.FirstOrDefault(m => m.Placa == placa && m.ClienteId == clienteId && m.Ativo);
        if (moto == null) throw new NotFoundException("Moto não encontrada ou não pertence ao cliente.");

        moto.Ativo = false;
        moto.DeletadoEm = DateTime.UtcNow;

        _context.SaveChanges();
    }

    // Método interno para uso do Worker
    internal async Task<List<Moto>> ListarMotosAtivasParaWorkerAsync(CancellationToken cancellationToken)
    {
        return await _context.Motos
            .Include(m => m.Cliente)
            .Include(m => m.Revisoes)
            .Where(m => m.Ativo && m.Cliente.Ativo)
            .ToListAsync(cancellationToken);
    }
}
