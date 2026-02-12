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
    private readonly ValidationService _validationService;

    public ConcessionariaService(AppDbContext context, IMapper mapper, ValidationService validationService)
    {
        _context = context;
        _mapper = mapper;
        _validationService = validationService;
    }

    public ConcessionariaResponse CriarConcessionaria(CriarConcessionariaRequest concessionariaRequest)
    {
        // Validar CNPJ, Telefone e Senha
        if (!_validationService.IsCnpj(concessionariaRequest.Cnpj))
            throw new DomainException("CNPJ inválido.");
        
        if (!_validationService.IsTelefone(concessionariaRequest.Telefone))
            throw new DomainException("Telefone inválido.");

        if (!_validationService.IsSenhaSegura(concessionariaRequest.Senha))
            throw new DomainException("A senha deve conter no mínimo 8 caracteres, incluindo letras maiúsculas, minúsculas, números e caracteres especiais.");

        // verificar se email ou cnpj já existe
        if (_context.Concessionarias.Any(c =>
                c.Email == concessionariaRequest.Email || c.Cnpj == concessionariaRequest.Cnpj))
            throw new ConflictException("Email ou CNPJ já cadastrado.");

        string senhaCriptografada = GeradorHash.HashPassword(concessionariaRequest.Senha);

        // criar concessionaria
        var concessionaria = _mapper.Map<Concessionaria>(concessionariaRequest);
        concessionaria.Senha = senhaCriptografada;
        concessionaria.Cnpj = _validationService.SomenteNumeros(concessionaria.Cnpj);
        concessionaria.Telefone = _validationService.SomenteNumeros(concessionaria.Telefone);

        _context.Add(concessionaria);
        _context.SaveChanges();
        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }

    public ConcessionariaResponse ObterConcessionariaPorId(int id)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }
    
    public List<ConcessionariaResponse> ListarConcessionarias()
    {
        var concessionarias = _context.Concessionarias.Include(c => c.Enderecos).Where(c => c.Ativo).ToList();
        return _mapper.Map<List<ConcessionariaResponse>>(concessionarias);
    }
    
    public ConcessionariaResponse AtualizarConcessionaria(int id, AtualizarConcessionariaRequest request)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        if (!string.IsNullOrEmpty(request.Telefone) && !_validationService.IsTelefone(request.Telefone))
            throw new DomainException("Telefone inválido.");

        if (!string.IsNullOrEmpty(request.Email) && concessionaria.Email != request.Email && _context.Concessionarias.Any(c => c.Email == request.Email))
            throw new ConflictException("Email já cadastrado.");

        _mapper.Map(request, concessionaria);

        if (request.Telefone != null)
            concessionaria.Telefone = _validationService.SomenteNumeros(concessionaria.Telefone);

        concessionaria.AtualizadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
        return _mapper.Map<ConcessionariaResponse>(concessionaria);
    }

    public void DeletarConcessionaria(int id)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        concessionaria.Ativo = false;
        concessionaria.DeletadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
    }

    public EnderecoResponse AdicionarEndereco(int id, AdicionarEnderecoRequest request)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .FirstOrDefault(c => c.Id == id && c.Ativo);

        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        var endereco = _mapper.Map<Endereco>(request);
        endereco.Cep = _validationService.SomenteNumeros(endereco.Cep);

        concessionaria.Enderecos.Add(endereco);
        _context.SaveChanges();

        return _mapper.Map<EnderecoResponse>(endereco);
    }

    public EnderecoResponse ObterEnderecoPorId(int concessionariaId, int enderecoId)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .FirstOrDefault(c => c.Id == concessionariaId && c.Ativo);

        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        var endereco = concessionaria.Enderecos.FirstOrDefault(e => e.Id == enderecoId);
        
        if (endereco == null) throw new NotFoundException("Endereço não encontrado ou não pertence a esta concessionária.");

        return _mapper.Map<EnderecoResponse>(endereco);
    }

    public void RemoverEndereco(int concessionariaId, int enderecoId)
    {
        var concessionaria = _context.Concessionarias
            .Include(c => c.Enderecos)
            .FirstOrDefault(c => c.Id == concessionariaId && c.Ativo);

        if (concessionaria == null) throw new NotFoundException("Concessionária não encontrada.");

        var endereco = concessionaria.Enderecos.FirstOrDefault(e => e.Id == enderecoId);
        
        if (endereco == null) throw new NotFoundException("Endereço não encontrado ou não pertence a esta concessionária.");

        concessionaria.Enderecos.Remove(endereco);
        _context.Enderecos.Remove(endereco); // Remove explicitamente da tabela de endereços
        _context.SaveChanges();
    }
}