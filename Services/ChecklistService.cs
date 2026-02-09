using Motos.Models;

namespace Motos.Services;

/// <summary>
/// Serviço para converter ChecklistTemplate + Catálogo de itens em RevisaoItem.
/// Responsável por "montar" os itens de uma revisão a partir de um template.
/// </summary>
public class ChecklistService
{
	private readonly ItemCatalogoService _itemCatalogoService;
	private readonly ChecklistTemplateService _checklistTemplateService;
	public ChecklistService(ItemCatalogoService itemCatalogoService, ChecklistTemplateService checklistTemplateService)
	{
		_itemCatalogoService = itemCatalogoService;
		_checklistTemplateService = checklistTemplateService;
	}
	
	/// <summary>
	/// Gera lista de RevisaoItem a partir de um ChecklistTemplate,
	/// buscando os dados de ItemCatalogo para cada item do template.
	/// </summary>
	///
	public List<RevisaoItem> GerarItensParaRevisao(ChecklistTemplate template)
	{
		var itensRevisao = new List<RevisaoItem>();

		foreach (var itemTemplate in template.Itens.OrderBy(i => i.Ordem))
		{
			var itemCatalogo = _itemCatalogoService.ObterPorId(itemTemplate.ItemCatalogoId);
			if (itemCatalogo == null)
			{
				Console.WriteLine($"⚠️  ItemCatalogo {itemTemplate.ItemCatalogoId} não encontrado no catálogo");
				continue;
			}

			var revisaoItem = itemTemplate.ParaRevisaoItem(itemCatalogo);
			itensRevisao.Add(revisaoItem);
		}

		return itensRevisao;
	}

	/// <summary>
	/// Busca um template por modelo e número de revisão, depois gera os itens.
	/// </summary>
	public List<RevisaoItem> GerarItensParaRevisao(string modelo, int numeroRevisao)
	{
		var template = _checklistTemplateService.ListarPorModelo(modelo)
			.FirstOrDefault(t => t.NumeroRevisao == numeroRevisao);

		if (template == null)
		{
			Console.WriteLine($"⚠️  Nenhum template encontrado para modelo '{modelo}' e revisão #{numeroRevisao}");
			return new();
		}

		return GerarItensParaRevisao(template);
	}

	/// <summary>
	/// Obtém um template pelo modelo e número de revisão.
	/// </summary>
	public ChecklistTemplate? ObterTemplatePara(string modelo, int numeroRevisao)
	{
		return _checklistTemplateService.ListarPorModelo(modelo)
			.FirstOrDefault(t => t.NumeroRevisao == numeroRevisao);
	}
}
