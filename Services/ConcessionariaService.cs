using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    private readonly ChecklistTemplateService _checklistTemplateService;

    public ConcessionariaService(AppDbContext context, IMapper mapper, ChecklistTemplateService checklistTemplateService)
    {
        _context = context;
        _mapper = mapper;
        _checklistTemplateService = checklistTemplateService;
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

        // associar todos os templates disponíveis no banco à nova concessionária
        var dbTemplates = _context.ChecklistTemplates.ToList();
        concessionaria.ChecklistTemplates = dbTemplates;

        _context.Add(concessionaria);
        _context.SaveChanges();
        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }

    public ConcessionariaResponse ObterConcessionariaPorId(int id)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .Include(c => c.ChecklistTemplates)
            .FirstOrDefault(c => c.Id == id && c.Ativo);
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

    public EnderecoResponse AdicionarEndereco(int id, AdicionarEnderecoRequest request)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .FirstOrDefault(c => c.Id == id && c.Ativo);

        if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");

        var endereco = _mapper.Map<Endereco>(request);

        concessionaria.Enderecos.Add(endereco);
        _context.SaveChanges();

        return _mapper.Map<EnderecoResponse>(endereco);
    }

    public EnderecoResponse ObterEnderecoPorId(int concessionariaId, int enderecoId)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .FirstOrDefault(c => c.Id == concessionariaId && c.Ativo);

        if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");

        var endereco = concessionaria.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

        if (endereco == null) throw new DomainException("Endereço não encontrado ou não pertence a esta concessionária.");

        return _mapper.Map<EnderecoResponse>(endereco);
    }

    public void RemoverEndereco(int concessionariaId, int enderecoId)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .FirstOrDefault(c => c.Id == concessionariaId && c.Ativo);

        if (concessionaria == null) throw new DomainException("Concessionária não encontrada.");

        var endereco = concessionaria.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

        if (endereco == null) throw new DomainException("Endereço não encontrado ou não pertence a esta concessionária.");

        concessionaria.Enderecos.Remove(endereco);
        _context.Enderecos.Remove(endereco); // Remove explicitamente da tabela de endereços
        _context.SaveChanges();
    }
}