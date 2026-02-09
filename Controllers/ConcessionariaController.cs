using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConcessionariaController : ControllerBase
{
    private readonly ConcessionariaService _service;

    public ConcessionariaController(ConcessionariaService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult CriarConcessionaria(CriarConcessionariaRequest request)
    {
        var response = _service.CriarConcessionaria(request);
        return CreatedAtAction(nameof(ObterConcessionaria), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpGet]
    public IActionResult ObterConcessionaria()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.ObterConcessionariaPorId(userId);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpPut]
    public IActionResult AtualizarConcessionaria(AtualizarConcessionariaRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.AtualizarConcessionaria(userId, request);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpDelete]
    public IActionResult DeletarConcessionaria()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        _service.DeletarConcessionaria(userId);
        return NoContent();
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpPost("enderecos")]
    public IActionResult AdicionarEndereco(AdicionarEnderecoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.AdicionarEndereco(userId, request);
        
        return CreatedAtAction(nameof(ObterEndereco), new { id = response.Id }, response);
    }
    
    [Authorize(Roles = "Concessionaria")]
    [HttpGet("enderecos/{id}")]
    public IActionResult ObterEndereco(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();
 
        var response = _service.ObterEnderecoPorId(userId, id);
        return Ok(response);
    }
    
    [Authorize(Roles = "Concessionaria")]
    [HttpDelete("enderecos/{id}")]
    public IActionResult DeletarEndereco(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();
 
        _service.RemoverEndereco(userId, id);
        return NoContent();
    }
}