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
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
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
}
