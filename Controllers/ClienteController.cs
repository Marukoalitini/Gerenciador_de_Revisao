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
        return CreatedAtAction(nameof(ObterCliente), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet]
    public IActionResult ObterCliente()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.ObterClientePorId(userId);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPut]
    public IActionResult AtualizarCliente(AtualizarClienteRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.AtualizarCliente(userId, request);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpDelete]
    public IActionResult DeletarCliente()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        _service.DeletarCliente(userId);
        return NoContent();
    }

    /// <summary>
    /// Define ou atualiza o endereço do cliente.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    [HttpPut("endereco")]
    public IActionResult DefinirEndereco(AdicionarEnderecoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.DefinirEndereco(userId, request);
        return Ok(response);
    }
}