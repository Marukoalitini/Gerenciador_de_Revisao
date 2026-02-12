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
    private readonly ValidacaoService _validacaoService;

    public ClienteService(AppDbContext context, IMapper mapper, ValidacaoService validacaoService)
    {
        _context = context;
        _mapper = mapper;
        _validacaoService = validacaoService;
    }

    public ClienteResponse CriarCliente(CriarClienteRequest clienteRequest)
    {
        // Validar CPF, Telefone e Senha
        if (!_validacaoService.IsCpf(clienteRequest.Cpf))
            throw new BadRequestException("CPF inválido.");
        
        if (!_validacaoService.IsTelefone(clienteRequest.Telefone))
            throw new BadRequestException("Telefone inválido.");

        if (!_validacaoService.IsSenhaSegura(clienteRequest.Senha))
            throw new BadRequestException("A senha deve conter no mínimo 8 caracteres, incluindo letras maiúsculas, minúsculas, números e caracteres especiais.");

        // verificar se email ou cpf já existe
        if (_context.Clientes.Any(c => c.Email == clienteRequest.Email || c.Cpf == clienteRequest.Cpf))
            throw new ConflictException("Email ou CPF já cadastrado.");

        string senhaCriptografada = GeradorHash.HashPassword(clienteRequest.Senha);

        // criar cliente
        var cliente = _mapper.Map<Cliente>(clienteRequest);
        cliente.Senha = senhaCriptografada;
        cliente.Cpf = _validacaoService.SomenteNumeros(cliente.Cpf);
        cliente.Telefone = _validacaoService.SomenteNumeros(cliente.Telefone);
        cliente.Celular = _validacaoService.SomenteNumeros(cliente.Celular);

        _context.Add(cliente);
        _context.SaveChanges();
        return _mapper.Map<ClienteResponse>(cliente);
    }

    public ClienteResponse ObterClientePorId(int id)
    {
        var cliente = _context.Clientes
            .Include(c => c.Motos.Where(m => m.Ativo))
            .Include(c => c.Endereco)
            .FirstOrDefault(c => c.Id == id && c.Ativo);
            
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        return _mapper.Map<ClienteResponse>(cliente);
    }

    public ClienteResponse AtualizarContato(int id, AtualizarClienteRequest request)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

        if (!string.IsNullOrEmpty(request.Telefone) && !_validacaoService.IsTelefone(request.Telefone))
            throw new BadRequestException("Telefone inválido.");

        if (!string.IsNullOrEmpty(request.Celular) && !_validacaoService.IsTelefone(request.Celular))
            throw new BadRequestException("Celular inválido.");

        if (!string.IsNullOrEmpty(request.Email) && cliente.Email != request.Email && _context.Clientes.Any(c => c.Email == request.Email))
            throw new ConflictException("Email já cadastrado.");

        _mapper.Map(request, cliente);
        
        if (request.Telefone != null)
            cliente.Telefone = _validacaoService.SomenteNumeros(cliente.Telefone);
        
        if (request.Celular != null)
            cliente.Celular = _validacaoService.SomenteNumeros(cliente.Celular);

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

    public EnderecoResponse AdicionarEndereco(int id, AdicionarEnderecoRequest request)
    {
        var cliente = _context.Clientes
            .Include(c => c.Endereco)
            .FirstOrDefault(c => c.Id == id && c.Ativo);

        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");
        if (cliente.Endereco != null) throw new ConflictException("Cliente já possui um endereço cadastrado.");

        cliente.Endereco = _mapper.Map<Endereco>(request);
        cliente.Endereco.Cep = _validacaoService.SomenteNumeros(cliente.Endereco.Cep);
        _context.SaveChanges();
        return _mapper.Map<EnderecoResponse>(cliente.Endereco);
    }

    public EnderecoResponse AtualizarEndereco(int id, AdicionarEnderecoRequest request)
    {
        var cliente = _context.Clientes
            .Include(c => c.Endereco)
            .FirstOrDefault(c => c.Id == id && c.Ativo);
        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");
        if (cliente.Endereco == null) throw new NotFoundException("Cliente não possui endereço cadastrado.");

        _mapper.Map(request, cliente.Endereco);
        cliente.Endereco.Cep = _validacaoService.SomenteNumeros(cliente.Endereco.Cep);
        _context.SaveChanges();
        return _mapper.Map<EnderecoResponse>(cliente.Endereco);
    }

    public void RemoverEndereco(int id)
    {
        var cliente = _context.Clientes
            .Include(c => c.Endereco)
            .FirstOrDefault(c => c.Id == id && c.Ativo);

        if (cliente == null) throw new NotFoundException("Cliente não encontrado.");
        if (cliente.Endereco == null) throw new NotFoundException("Cliente não possui endereço cadastrado.");

        _context.Enderecos.Remove(cliente.Endereco);
        cliente.Endereco = null;
        _context.SaveChanges();
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
        _context.Motos.Add(moto);
        _context.SaveChanges();

        return _mapper.Map<MotoComRevisoesResponse>(moto);
    }

    public List<MotoComRevisoesResponse> ObterMotosDoCliente(int clienteId)
    {
        var motos = _context.Motos
            .Where(m => m.ClienteId == clienteId && m.Ativo)
            .ToList();

        return _mapper.Map<List<MotoComRevisoesResponse>>(motos);
    }

    public MotoComRevisoesResponse ObterMotoPorId(int clienteId, int motoId)
    {
        var moto = _context.Motos.FirstOrDefault(m => m.Id == motoId && m.ClienteId == clienteId && m.Ativo);
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
}