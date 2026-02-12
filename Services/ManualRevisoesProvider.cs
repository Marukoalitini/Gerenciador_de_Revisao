using System.Text.Json;
using System.Text.Json.Serialization;
using Motos.Enums;
using Motos.Models;

namespace Motos.Services;

public class ManualRevisoesProvider
{
	private readonly List<TemplateRevisao> _templates = [];
	private readonly Dictionary<int, CatalogoItem> _catalogoPorId = new();

	public ManualRevisoesProvider()
	{
		Carregar();
	}

	public void Carregar()
	{
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			Converters = { new JsonStringEnumConverter() }
		};

		var basePath = Directory.GetCurrentDirectory();
		var templatesPath = Path.Combine(basePath, "Utils", "checklists-template.json");
		var catalogoPath = Path.Combine(basePath, "Utils", "itens-catalogo.json");

		if (!File.Exists(templatesPath))
			throw new FileNotFoundException($"Arquivo de templates nao encontrado: {templatesPath}");

		if (!File.Exists(catalogoPath))
			throw new FileNotFoundException($"Arquivo de catalogo nao encontrado: {catalogoPath}");

		var templatesJson = File.ReadAllText(templatesPath);
		var catalogoJson = File.ReadAllText(catalogoPath);

		var templates = JsonSerializer.Deserialize<List<TemplateRevisao>>(templatesJson, options) ?? new();
		var catalogo = JsonSerializer.Deserialize<List<CatalogoItem>>(catalogoJson, options) ?? new();

		_templates.Clear();
		_templates.AddRange(NormalizarTemplates(templates));

		_catalogoPorId.Clear();
		foreach (var item in catalogo)
		{
			_catalogoPorId[item.Id] = item;
		}
	}

	public List<Revisao> ObterRevisoesPara(ModeloMoto modelo)
	{
		var revisoes = new List<Revisao>();

		foreach (var template in _templates
					 .Where(t => t.Modelos.Contains(modelo))
					 .OrderBy(t => t.NumeroRevisao))
		{
			var itens = template.Itens
				.OrderBy(i => i.Ordem)
				.Select(CriarItem)
				.ToList();

			var valorTotal = itens.Sum(i => i.Valor ?? 0.0);

			revisoes.Add(new Revisao
			{
				Numero = template.NumeroRevisao,
				Itens = itens,
				ValorTotal = valorTotal
			});
		}

		return revisoes;
	}

	private static List<TemplateRevisao> NormalizarTemplates(List<TemplateRevisao> templates)
	{
		foreach (var template in templates)
		{
			var modelos = new List<ModeloMoto>();
			foreach (var modeloStr in template.ModelosRaw)
			{
				if (Enum.TryParse<ModeloMoto>(modeloStr, true, out var modelo))
					modelos.Add(modelo);
			}

			template.Modelos = modelos;
		}

		return templates;
	}

	private RevisaoItem CriarItem(TemplateItem item)
	{
		_catalogoPorId.TryGetValue(item.ItemCatalogoId, out var catalogo);

		var valor = item.ValorSugerido ?? catalogo?.Preco;
		var valorDouble = valor.HasValue ? Convert.ToDouble(valor.Value) : (double?)null;

		return new RevisaoItem
		{
			Tipo = catalogo?.Tipo ?? TipoItemRevisao.Peca,
			Descricao = catalogo?.Descricao ?? $"ItemCatalogoId:{item.ItemCatalogoId}",
			Grupo = item.Grupo,
			Ordem = item.Ordem,
			Valor = valorDouble,
			Quantidade = null,
			Realizado = true
		};
	}

	private sealed class TemplateRevisao
	{
		public int NumeroRevisao { get; set; }

		[JsonPropertyName("Modelos")]
		public List<string> ModelosRaw { get; set; } = new();

		public List<TemplateItem> Itens { get; set; } = new();

		[JsonIgnore]
		public List<ModeloMoto> Modelos { get; set; } = new();
	}

	private sealed class TemplateItem
	{
		public int ItemCatalogoId { get; set; }
		public string? Grupo { get; set; }
		public int Ordem { get; set; }
		public decimal? ValorSugerido { get; set; }
	}

	private sealed class CatalogoItem
	{
		public int Id { get; set; }
		public TipoItemRevisao Tipo { get; set; }
		public string Descricao { get; set; } = string.Empty;
		public decimal? Preco { get; set; }
	}
}
