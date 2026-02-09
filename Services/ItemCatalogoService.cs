using System.Text.Json;
using System.Text.Json.Serialization;
using Motos.Enums;
using Motos.Models;

namespace Motos.Services;

public class ItemCatalogoService
{
	private List<ItemCatalogo> _itens = [];

	public ItemCatalogoService()
	{
		Inicializar();
	}

	public void Inicializar()
	{
		try
		{
			string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Utils", "itens-catalogo.json");

			if (!File.Exists(caminhoArquivo))
				throw new FileNotFoundException($"Arquivo de catálogo não encontrado: {caminhoArquivo}");

			string json = File.ReadAllText(caminhoArquivo);
			var opcoesJson = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				Converters = { new JsonStringEnumConverter() }
			};
			var itensCarregados = JsonSerializer.Deserialize<List<ItemCatalogo>>(json, opcoesJson);

			_itens = itensCarregados ?? new();
			Console.WriteLine($"✓ {_itens.Count} itens do catálogo carregados com sucesso.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"✗ Erro ao carregar catálogo de itens: {ex.Message}");
			throw;
		}
	}

	public ItemCatalogo? ObterPorId(int id)
	{
		return _itens.FirstOrDefault(i => i.Id == id);
	}

	public List<ItemCatalogo> ListarTodos()
	{
		return new List<ItemCatalogo>(_itens);
	}

	public List<ItemCatalogo> ListarPorTipo(TipoItemRevisao tipo)
	{
		return _itens.Where(i => i.Tipo == tipo).ToList();
	}
}
