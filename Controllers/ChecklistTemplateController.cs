using Microsoft.AspNetCore.Mvc;
using Motos.Dto.Response;
using Motos.Enums;
using Motos.Models;
using Motos.Services;

namespace Motos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChecklistTemplateController : ControllerBase
{
	[HttpGet]
	public ActionResult<IEnumerable<ChecklistTemplateResponse>> GetAll()
	{
		var templates = ChecklistTemplateService.ListarTodos();
		var dtos = templates.Select(t => ToDto(t)).ToList();
		return Ok(dtos);
	}

	[HttpGet("{id}")]
	public ActionResult<ChecklistTemplateResponse> GetById(int id)
	{
		var template = ChecklistTemplateService.ObterPorId(id);
		if (template == null) return NotFound();
		return Ok(ToDto(template));
	}

	/// <summary>
	/// Gera os itens de revisão (RevisaoItem) para um template específico.
	/// Combina dados do template com o catálogo de itens.
	/// </summary>
	[HttpGet("{id}/itens")]
	public ActionResult<List<RevisaoItem>> GerarItensRevisao(int id)
	{
		var template = ChecklistTemplateService.ObterPorId(id);
		if (template == null) return NotFound(new { mensagem = "Template não encontrado" });

		var itens = ChecklistService.GerarItensParaRevisao(template);
		return Ok(itens);
	}

	private static ChecklistTemplateResponse ToDto(ChecklistTemplate t)
	{
		var itens = t.Itens.Select(i => new ItemTemplateResponse(
			i.Id,
			i.ItemCatalogoId,
			i.Grupo,
			i.Ordem,
			i.ValorSugerido
		)).ToList();

		var modelosDisplay = t.Modelos
			.Select(m => Enum.TryParse<ModeloMoto>(m, out var modelo)
				? modelo.GetDisplayName()
				: m)
			.ToList();

		return new ChecklistTemplateResponse(
			t.Id,
			t.NumeroRevisao,
			modelosDisplay,
			itens
		);
	}
}
