using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Services;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgendamentoController : ControllerBase
{
    private readonly AgendamentoService _service;

    public AgendamentoController(AgendamentoService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("proxima/{placa}")]
    public async Task<IActionResult> ObterProximaRevisao(string placa)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = await _service.ObterProximaRevisaoAsync(userId, placa);
        if (response == null) return NotFound(new { mensagem = "Nenhuma revisão pendente encontrada." });

        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("solicitar/{placa}")]
    public async Task<IActionResult> SolicitarAgendamento(string placa, SolicitarAgendamentoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = await _service.SolicitarAgendamentoAsync(placa, userId, request);
        return Ok(response);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("reagendar/{placa}")]
    public async Task<IActionResult> ReagendarPeloCliente(string placa, [FromBody] AgendarRevisaoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = await _service.ReagendarAgendamentoAsync(placa, userId, request);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpGet("solicitacoes")]
    public async Task<IActionResult> ListarSolicitacoes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = await _service.ListarSolicitacoesAsync(userId);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpPut("confirmar/{revisaoId}")]
    public async Task<IActionResult> ConfirmarAgendamento(int revisaoId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = await _service.ConfirmarAgendamentoAsync(revisaoId, userId);
        return Ok(response);
    }

    [Authorize(Roles = "Concessionaria")]
    [HttpPut("recusar/{revisaoId}")]
    public async Task<IActionResult> RecusarAgendamento(int revisaoId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId <= 0) return Forbid();

        var response = await _service.RecusarAgendamentoAsync(revisaoId, userId);
        return Ok(response);
    }
}
