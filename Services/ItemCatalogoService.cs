using System.Text.Json;
using System.Text.Json.Serialization;
using Motos.Enums;
using Motos.Models;

namespace Motos.Services;

public static class ItemCatalogoService
{
	private static List<ItemCatalogo> Itens = new();

	public static void Inicializar()
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

			Itens = itensCarregados ?? new();
			Console.WriteLine($"✓ {Itens.Count} itens do catálogo carregados com sucesso.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"✗ Erro ao carregar catálogo de itens: {ex.Message}");
			throw;
		}
	}

	public static ItemCatalogo? ObterPorId(int id)
	{
		return Itens.FirstOrDefault(i => i.Id == id);
	}

	public static List<ItemCatalogo> ListarTodos()
	{
		return new List<ItemCatalogo>(Itens);
	}

	public static List<ItemCatalogo> ListarPorTipo(TipoItemRevisao tipo)
	{
		return Itens.Where(i => i.Tipo == tipo).ToList();
	}
}
