using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RevisaoController : ControllerBase
{
	private readonly RevisaoService _service;
	private readonly IMapper _mapper;

	public RevisaoController(RevisaoService service, IMapper mapper)
	{
		_service = service;
		_mapper = mapper;
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

		var revisao = await _service.CriarRevisaoAsync(request);

		var response = _mapper.Map<RevisaoResponse>(revisao);

		return CreatedAtAction(nameof(ObterPorId), new { id = revisao.Id }, response);
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> Listar([FromQuery] int? concessionariaId, [FromQuery] int? clienteId)
	{
		var list = await _service.ListarAsync(concessionariaId, clienteId);
		var resp = _mapper.Map<List<RevisaoResponse>>(list);

	return Ok(resp);
	}

	[Authorize]
	[HttpGet("{id}")]
	public async Task<IActionResult> ObterPorId(int id)
	{
		var r = await _service.ObterPorIdAsync(id);
		if (r == null) return NotFound();

		var resp = _mapper.Map<RevisaoResponse>(r);

		return Ok(resp);
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
