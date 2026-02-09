using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AutenticacaoController : ControllerBase
{
    private readonly AutenticacaoService _autenticacaoService;

    public AutenticacaoController(AutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }
    
    
    // POST: api/Autenticacao/Cliente
    [HttpPost("Cliente/Login")]
    public IActionResult LoginCliente(LoginRequest loginRequest)
    {
        var loginResponse = _autenticacaoService.LoginCliente(loginRequest);

        return Ok(loginResponse);
    }
    

    [HttpPost("Concessionaria/Login")]
    public IActionResult LoginConcessionaria(LoginRequest loginRequest)
    {
        var loginResponse = _autenticacaoService.LoginConcessionaria(loginRequest);

        return Ok(loginResponse);
    }

}