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
    public IActionResult AtualizarContato(AtualizarClienteRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.AtualizarContato(userId, request);
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

    [Authorize(Roles = "Cliente")]
    [HttpPost("motos")]
    public IActionResult AdicionarMoto(MotoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.CadastrarMoto(userId, request);
        return CreatedAtAction(nameof(ObterMoto), new { id = response.Id }, response);
    }
    
    [Authorize(Roles = "Cliente")]
    [HttpPut("motos")]
    public IActionResult EditarMoto(string placa, AtualizarMotoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.EditarMoto(userId, placa, request);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("motos")]
    public IActionResult ObterMotos()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.ObterMotosDoCliente(userId);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("motos/{id}")]
    public IActionResult ObterMoto(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _service.ObterMotoPorId(userId, id);
        return Ok(response);
    }
}