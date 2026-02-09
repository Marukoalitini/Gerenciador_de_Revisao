using AutoMapper;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Exceptions;
using Motos.Models;
using GeradorHash = BCrypt.Net.BCrypt;

namespace Motos.Services;

public class ConcessionariaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ConcessionariaService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public ConcessionariaResponse CriarConcessionaria(CriarConcessionariaRequest concessionariaRequest)
    {
        // verificar se email ou cnpj já existe
        if (_context.Concessionarias.Any(c =>
                c.Email == concessionariaRequest.Email || c.Cnpj == concessionariaRequest.Cnpj))
            throw new DomainException("Email ou CNPJ já cadastrado.");

        string senhaCriptografada = GeradorHash.HashPassword(concessionariaRequest.Senha);

        // criar concessionaria
        var concessionaria = _mapper.Map<Concessionaria>(concessionariaRequest);
        concessionaria.Senha = senhaCriptografada;

        _context.Add(concessionaria);
        _context.SaveChanges();
        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }

    public ConcessionariaResponse ObterConcessionariaPorId(int id)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");

        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }

    public ConcessionariaResponse AtualizarConcessionaria(int id, AtualizarConcessionariaRequest request)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");

        if (!string.IsNullOrEmpty(request.Email) && concessionaria.Email != request.Email && _context.Concessionarias.Any(c => c.Email == request.Email))
            throw new DomainException("Email já cadastrado.");

        _mapper.Map(request, concessionaria);
        concessionaria.AtualizadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }

    public void DeletarConcessionaria(int id)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");

        concessionaria.Ativo = false;
        concessionaria.DeletadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
    }
}