using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Models;
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

		var revisao = await _service.CriarRevisaoAsync(request);
		ConcessionariaResponse? concessionariaResp = null;
		if (revisao.ConcessinariaResposavel != null)
		{
			concessionariaResp = new ConcessionariaResponse(
				revisao.ConcessinariaResposavel.Id,
				revisao.ConcessinariaResposavel.Nome,
				revisao.ConcessinariaResposavel.Telefone,
				revisao.ConcessinariaResposavel.Cnpj,
				revisao.ConcessinariaResposavel.Enderecos?.Select(e => new EnderecoResponse(e.Id, e.Rua, e.Numero, e.Bairro, e.Cidade, e.Estado, e.Cep, e.Complemento)).ToArray() ?? Array.Empty<EnderecoResponse>()
			);
		}

		var response = new RevisaoResponse(
			revisao.Id,
			revisao.Numero,
			new ClienteResponse(revisao.Cliente?.Id ?? revisao.ClienteId, revisao.Cliente?.Nome ?? string.Empty, revisao.Cliente?.Email ?? string.Empty, revisao.Cliente?.Telefone ?? string.Empty, revisao.Cliente?.Celular ?? string.Empty),
			new MotoResponse(revisao.Moto?.Id ?? revisao.MotoId, revisao.Moto?.Cor ?? string.Empty, revisao.Moto?.NumeroChassi ?? string.Empty, revisao.Moto?.Placa ?? string.Empty, revisao.Moto?.DataDeVenda ?? DateTime.MinValue, revisao.Moto?.NotaFiscal ?? string.Empty, revisao.Moto?.Serie ?? 0, revisao.Moto?.ImgDecalqueChassi ?? string.Empty),
			revisao.Status,
			revisao.KmMaximo,
			revisao.KmTolerancia,
			revisao.TempoMaximo,
			revisao.TempoTolerancia,
			revisao.KmAtual,
			revisao.DataRevisao,
			revisao.ValorTotal,
			revisao.NotaDeServico,
			concessionariaResp,
			revisao.Itens.Select(i => new RevisaoItemResponse(i.Id, i.Tipo, i.Descricao, i.Grupo, i.Ordem, i.Valor, i.Quantidade, i.Realizado)).ToArray()
		);

		return CreatedAtAction(nameof(ObterPorId), new { id = revisao.Id }, response);
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> Listar([FromQuery] int? concessionariaId, [FromQuery] int? clienteId)
	{
		var list = await _service.ListarAsync(concessionariaId, clienteId);
		var resp = list.Select(r =>
		{
			ConcessionariaResponse? concessionariaResp = null;
			if (r.ConcessinariaResposavel != null)
			{
				concessionariaResp = new ConcessionariaResponse(
					r.ConcessinariaResposavel.Id,
					r.ConcessinariaResposavel.Nome,
					r.ConcessinariaResposavel.Telefone,
					r.ConcessinariaResposavel.Cnpj,
					r.ConcessinariaResposavel.Enderecos?.Select(e => new EnderecoResponse(e.Id, e.Rua, e.Numero, e.Bairro, e.Cidade, e.Estado, e.Cep, e.Complemento)).ToArray() ?? Array.Empty<EnderecoResponse>()
				);
			}

			return new RevisaoResponse(
				r.Id,
				r.Numero,
				new ClienteResponse(r.Cliente?.Id ?? r.ClienteId, r.Cliente?.Nome ?? string.Empty, r.Cliente?.Email ?? string.Empty, r.Cliente?.Telefone ?? string.Empty, r.Cliente?.Celular ?? string.Empty),
				new MotoResponse(r.Moto?.Id ?? r.MotoId, r.Moto?.Cor ?? string.Empty, r.Moto?.NumeroChassi ?? string.Empty, r.Moto?.Placa ?? string.Empty, r.Moto?.DataDeVenda ?? DateTime.MinValue, r.Moto?.NotaFiscal ?? string.Empty, r.Moto?.Serie ?? 0, r.Moto?.ImgDecalqueChassi ?? string.Empty),
				r.Status,
				r.KmMaximo,
				r.KmTolerancia,
				r.TempoMaximo,
				r.TempoTolerancia,
				r.KmAtual,
				r.DataRevisao,
				r.ValorTotal,
				r.NotaDeServico,
				concessionariaResp,
				r.Itens.Select(i => new RevisaoItemResponse(i.Id, i.Tipo, i.Descricao, i.Grupo, i.Ordem, i.Valor, i.Quantidade, i.Realizado)).ToArray()
			);
		}).ToArray();

		return Ok(resp);
	}

	[Authorize]
	[HttpGet("{id}")]
	public async Task<IActionResult> ObterPorId(int id)
	{
		var r = await _service.ObterPorIdAsync(id);
		if (r == null) return NotFound();

		ConcessionariaResponse? concessionariaResp = null;
		if (r.ConcessinariaResposavel != null)
		{
			concessionariaResp = new ConcessionariaResponse(
				r.ConcessinariaResposavel.Id,
				r.ConcessinariaResposavel.Nome,
				r.ConcessinariaResposavel.Telefone,
				r.ConcessinariaResposavel.Cnpj,
				r.ConcessinariaResposavel.Enderecos?.Select(e => new EnderecoResponse(e.Id, e.Rua, e.Numero, e.Bairro, e.Cidade, e.Estado, e.Cep, e.Complemento)).ToArray() ?? Array.Empty<EnderecoResponse>()
			);
		}

		var resp = new RevisaoResponse(
			r.Id,
			r.Numero,
			new ClienteResponse(r.Cliente?.Id ?? r.ClienteId, r.Cliente?.Nome ?? string.Empty, r.Cliente?.Email ?? string.Empty, r.Cliente?.Telefone ?? string.Empty, r.Cliente?.Celular ?? string.Empty),
			new MotoResponse(r.Moto?.Id ?? r.MotoId, r.Moto?.Cor ?? string.Empty, r.Moto?.NumeroChassi ?? string.Empty, r.Moto?.Placa ?? string.Empty, r.Moto?.DataDeVenda ?? DateTime.MinValue, r.Moto?.NotaFiscal ?? string.Empty, r.Moto?.Serie ?? 0, r.Moto?.ImgDecalqueChassi ?? string.Empty),
			r.Status,
			r.KmMaximo,
			r.KmTolerancia,
			r.TempoMaximo,
			r.TempoTolerancia,
			r.KmAtual,
			r.DataRevisao,
			r.ValorTotal,
			r.NotaDeServico,
			concessionariaResp,
			r.Itens.Select(i => new RevisaoItemResponse(i.Id, i.Tipo, i.Descricao, i.Grupo, i.Ordem, i.Valor, i.Quantidade, i.Realizado)).ToArray()
		);

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
