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
        return CreatedAtAction(nameof(CriarConcessionaria), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpGet("{id}")]
    public IActionResult ObterConcessionaria(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId != id) return Forbid();

        var response = _service.ObterConcessionariaPorId(id);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpPut("{id}")]
    public IActionResult AtualizarConcessionaria(int id, AtualizarConcessionariaRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId != id) return Forbid();

        var response = _service.AtualizarConcessionaria(id, request);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpDelete("{id}")]
    public IActionResult DeletarConcessionaria(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId != id) return Forbid();

        _service.DeletarConcessionaria(id);
        return NoContent();
    }
}