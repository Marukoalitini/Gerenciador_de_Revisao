using Motos.Data;
using Motos.Dto.Request;
using Motos.Dto.Response;
using GeradorHash = BCrypt.Net.BCrypt;

namespace Motos.Services;

public class AutenticacaoService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AutenticacaoService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public LoginResponse LoginCliente(LoginRequest loginRequest)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.Email == loginRequest.Email && c.Ativo);
        
        if (cliente == null || !GeradorHash.Verify(loginRequest.Senha, cliente.Senha))
            throw new UnauthorizedAccessException("Email ou Senha incorretos");

        var token = _tokenService.GerarToken(cliente.Id, cliente.NomeCliente, cliente.Email, "Cliente");
        
        return new LoginResponse(cliente.NomeCliente, cliente.Email, token);
    }

    public LoginResponse LoginConcessionaria(LoginRequest loginRequest)
    {
        var concessionaria = _context.Concessionarias.FirstOrDefault(c => c.Email == loginRequest.Email && c.Ativo);
        
        if (concessionaria == null || !GeradorHash.Verify(loginRequest.Senha, concessionaria.Senha))
            throw new UnauthorizedAccessException("Email ou Senha incorretos");

        var token = _tokenService.GerarToken(concessionaria.Id, concessionaria.NomeConcessionaria, concessionaria.Email, "Concessionaria");

        return new LoginResponse(concessionaria.NomeConcessionaria, concessionaria.Email, token);
    }
}