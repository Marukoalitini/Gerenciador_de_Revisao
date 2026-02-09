using AutoMapper;
using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Exceptions;
using Motos.Models;
using GeradorHash = BCrypt.Net.BCrypt;

namespace Motos.Services;

public class ClienteService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ClienteService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public ClienteResponse CriarCliente(CriarClienteRequest clienteRequest)
    {
        // verificar se email ou cpf já existe
        if (_context.Clientes.Any(c => c.Email == clienteRequest.Email || c.Cpf == clienteRequest.Cpf))
            throw new DomainException("Email ou CPF já cadastrado.");

        string senhaCriptografada = GeradorHash.HashPassword(clienteRequest.Senha);

        // criar cliente
        var cliente = _mapper.Map<Cliente>(clienteRequest);
        cliente.Senha = senhaCriptografada;

        _context.Add(cliente);
        _context.SaveChanges();
        return _mapper.Map<ClienteResponse>(cliente);
    }

    public ClienteResponse ObterClientePorId(int id)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new DomainException("Cliente não encontrado.");

        return _mapper.Map<ClienteResponse>(cliente);
    }

    public ClienteResponse AtualizarCliente(int id, AtualizarClienteRequest request)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new DomainException("Cliente não encontrado.");

        if (!string.IsNullOrEmpty(request.Email) && cliente.Email != request.Email && _context.Clientes.Any(c => c.Email == request.Email))
            throw new DomainException("Email já cadastrado.");

        _mapper.Map(request, cliente);
        cliente.AtualizadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
        return _mapper.Map<ClienteResponse>(cliente);
    }

    public void DeletarCliente(int id)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new DomainException("Cliente não encontrado.");

        cliente.Ativo = false;
        cliente.DeletadoEm = DateTime.UtcNow;
        
        _context.SaveChanges();
    }
}