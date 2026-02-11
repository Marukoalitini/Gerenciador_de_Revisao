using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MotoController : ControllerBase
{
    private readonly MotoService _motoService;

    public MotoController(MotoService motoService)
    {
        _motoService = motoService;
    }
    
    [Authorize(Roles = "Cliente")]
    [HttpPost("motos")]
    public IActionResult AdicionarMoto(MotoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _motoService.CadastrarMoto(userId, request);
        return CreatedAtAction(nameof(ObterMoto), new { id = response.Id }, response);
    }
    
    [Authorize(Roles = "Cliente")]
    [HttpPut("motos")]
    public IActionResult EditarMoto(string placa, AtualizarMotoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _motoService.EditarMoto(userId, placa, request);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("motos")]
    public IActionResult ObterMotos()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _motoService.ObterMotosDoCliente(userId);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("motos/{id}")]
    public IActionResult ObterMoto(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = _motoService.ObterMotoPorId(userId, id);
        return Ok(response);
    }
}