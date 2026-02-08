using System.Text.Json;
using Motos.Models;

namespace Motos.Services;

public static class RegraRevisaoService
{
	private static List<RegraRevisao> Regras = new();

	public static void Inicializar()
	{
		try
		{
			string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Utils", "regras-revisao.json");
			
			if (!File.Exists(caminhoArquivo))
				throw new FileNotFoundException($"Arquivo de regras não encontrado: {caminhoArquivo}");

			string json = File.ReadAllText(caminhoArquivo);
			var opcoesJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var regrasCarregadas = JsonSerializer.Deserialize<List<RegraRevisao>>(json, opcoesJson);

			Regras = regrasCarregadas ?? new();
			Console.WriteLine($"✓ {Regras.Count} regras de revisão carregadas com sucesso.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"✗ Erro ao carregar regras de revisão: {ex.Message}");
			throw;
		}
	}

	public static RegraRevisao? ObterPorNumero(int numeroRevisao)
	{
		return Regras.FirstOrDefault(r => r.NumeroRevisao == numeroRevisao);
	}

	public static List<RegraRevisao> ListarTodas()
	{
		return new List<RegraRevisao>(Regras);
	}
}
