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
	[HttpPost]
	public async Task<IActionResult> CriarRevisao(RevisaoRequest request)
	{
		// usuário autenticado deverá ser o cliente (ou concessionaria quando aplicável)
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
		if (userId <= 0) return Forbid();

		if (request.ClienteId != userId && !User.IsInRole("Concessionaria"))
			return Forbid();

		var response = await _service.CriarRevisaoAsync(request);

		return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
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

	[Authorize(Roles = "Concessionaria")]
	[HttpPut("{id}/executar")]
	public async Task<IActionResult> Executar(int id)
	{
		await _service.ExecutarRevisaoAsync(id);
		return NoContent();
	}

	[Authorize]
	[HttpDelete("{id}")]
	public async Task<IActionResult> Deletar(int id)
	{
		await _service.DeletarAsync(id);
		return NoContent();
	}
}
