using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Exceptions;
using Motos.Models;

namespace Motos.Services;

public class MotoService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public MotoService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public MotoResponse CadastrarMoto(int clienteId, MotoRequest request)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == clienteId && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");
        
        // Verificar se já existe moto com esse chassi ou placa
        if (_context.Motos.Any(m => m.NumeroChassi == request.NumeroChassi || m.Placa == request.Placa))
            throw new ConflictException("Moto com este chassi ou placa já cadastrada.");

        var moto = _mapper.Map<Moto>(request);
        moto.ClienteId = clienteId;
        _context.Motos.Add(moto);
        _context.SaveChanges();

        return _mapper.Map<MotoResponse>(moto);
    }

    public List<MotoResponse> ObterMotosDoCliente(int clienteId)
    {
        var motos = _context.Motos
            .Where(m => m.ClienteId == clienteId && m.Ativo)
            .ToList();

        return _mapper.Map<List<MotoResponse>>(motos);
    }

    public MotoResponse ObterMotoPorId(int clienteId, int motoId)
    {
        var moto = _context.Motos.FirstOrDefault(m => m.Id == motoId && m.ClienteId == clienteId && m.Ativo);
        if (moto == null) throw new NotFoundException("Moto não encontrada ou não pertence ao cliente.");

        return _mapper.Map<MotoResponse>(moto);
    }

    public MotoResponse EditarMoto(int clienteId, string placa, AtualizarMotoRequest request)
    {
        var moto = _context.Motos.FirstOrDefault(m => m.Placa == placa && m.ClienteId == clienteId && m.Ativo);
        if (moto == null) throw new NotFoundException("Moto não encontrada ou não pertence ao cliente.");

        _mapper.Map(request, moto);
        
        _context.SaveChanges();

        return _mapper.Map<MotoResponse>(moto);
    }

    public void RemoverMoto(int clienteId, string placa)
    {
        var moto = _context.Motos.FirstOrDefault(m => m.Placa == placa && m.ClienteId == clienteId && m.Ativo);
        if (moto == null) throw new NotFoundException("Moto não encontrada ou não pertence ao cliente.");

        moto.Ativo = false;
        moto.DeletadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
    }

    public Revisao? ObterUltimaRevisaoExecutada(int motoId)
    {
        var moto = _context.Motos.Include(m => m.Revisoes).FirstOrDefault(m => m.Id == motoId && m.Ativo);
        if (moto == null) return null;

        return moto.Revisoes
            .Where(r => r.Status == Enums.StatusRevisao.Executada)
            .OrderByDescending(r => r.DataExecucao ?? DateTime.MinValue)
            .ThenByDescending(r => r.Numero)
            .FirstOrDefault();
    }

    public Revisao? ObterRevisao(int numero)
    {
        var revisao = _context.Revisoes.Include(r => r.Moto).FirstOrDefault(r => r.Numero == numero);
        return revisao;
    }

}