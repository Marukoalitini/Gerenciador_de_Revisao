using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly ClienteService _service;

    public ClienteController(ClienteService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult CriarCliente(CriarClienteRequest request)
    {
        var response = _service.CriarCliente(request);
        return CreatedAtAction(nameof(CriarCliente), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("{id}")]
    public IActionResult ObterCliente(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId != id) return Forbid();

        var response = _service.ObterClientePorId(id);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPut("{id}")]
    public IActionResult AtualizarCliente(int id, AtualizarClienteRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId != id) return Forbid();

        var response = _service.AtualizarCliente(id, request);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpDelete("{id}")]
    public IActionResult DeletarCliente(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId != id) return Forbid();

        _service.DeletarCliente(id);
        return NoContent();
    }
}