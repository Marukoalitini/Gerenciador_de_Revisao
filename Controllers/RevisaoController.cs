using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RevisaoController : ControllerBase
{
	private readonly RevisaoService _service;

	public RevisaoController(RevisaoService service)
	{
		_service = service;
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> Listar([FromQuery] int? concessionariaId, [FromQuery] int? clienteId)
	{
		var list = await _service.ListarAsync(concessionariaId, clienteId);
		return Ok(list);
	}

	[Authorize]
	[HttpGet("{id}")]
	public async Task<IActionResult> ObterPorId(int id)
	{
		var response = await _service.ObterPorIdAsync(id);
		return Ok(response);
	}

    [Authorize]
    [HttpGet("moto/{motoId}/ultima-executada")]
    public async Task<IActionResult> ObterUltimaRevisaoExecutada(int motoId)
    {
        var response = await _service.ObterUltimaRevisaoExecutadaAsync(motoId);
        if (response == null) return NotFound(new { mensagem = "Nenhuma revisão executada encontrada para esta moto." });
        return Ok(response);
    }

	[Authorize(Roles = "Concessionaria")]
	[HttpPut("{id}/executar")]
	public async Task<IActionResult> Executar(int id)
	{
		await _service.ExecutarRevisaoAsync(id);
		return NoContent();
	}
}
