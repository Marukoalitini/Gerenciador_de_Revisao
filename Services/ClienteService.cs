using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            throw new ConflictException("Email ou CPF já cadastrado.");

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
        var cliente = _context.Clientes.Include(c => c.Motos).FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        return _mapper.Map<ClienteResponse>(cliente);
    }

    public ClienteResponse AtualizarContato(int id, AtualizarClienteRequest request)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        if (!string.IsNullOrEmpty(request.Email) && cliente.Email != request.Email && _context.Clientes.Any(c => c.Email == request.Email))
            throw new ConflictException("Email já cadastrado.");

        _mapper.Map(request, cliente);
        cliente.AtualizadoEm = DateTime.UtcNow;

        _context.SaveChanges();
        return _mapper.Map<ClienteResponse>(cliente);
    }

    public void DeletarCliente(int id)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        cliente.Ativo = false;
        cliente.DeletadoEm = DateTime.UtcNow;

        _context.SaveChanges();
    }

    public EnderecoResponse DefinirEndereco(int id, AdicionarEnderecoRequest request)
    {
        var cliente = _context.Clientes
            .Include(c => c.Endereco)
            .FirstOrDefault(c => c.Id == id && c.Ativo);

        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        // Se o cliente ainda não tem endereço, instanciamos um novo
        if (cliente.Endereco == null)
        {
            cliente.Endereco = _mapper.Map<Endereco>(request);
        }
        else
        {
            _mapper.Map(request, cliente.Endereco);
        }

        _context.SaveChanges();
        return _mapper.Map<EnderecoResponse>(cliente.Endereco);
    }
}
