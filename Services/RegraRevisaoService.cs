using System.Text.Json;
using Motos.Models;

namespace Motos.Services;

public class RegraRevisaoService
{
	private List<RegraRevisao> _regras = [];

	public RegraRevisaoService()
	{
		Inicializar();
	}

	public void Inicializar()
	{
		try
		{
			string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Utils", "regras-revisao.json");
			
			if (!File.Exists(caminhoArquivo))
				throw new FileNotFoundException($"Arquivo de regras não encontrado: {caminhoArquivo}");

			string json = File.ReadAllText(caminhoArquivo);
			var opcoesJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var regrasCarregadas = JsonSerializer.Deserialize<List<RegraRevisao>>(json, opcoesJson);

			_regras = regrasCarregadas ?? new();
			Console.WriteLine($"✓ {_regras.Count} regras de revisão carregadas com sucesso.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"✗ Erro ao carregar regras de revisão: {ex.Message}");
			throw;
		}
	}

	public RegraRevisao? ObterPorNumero(int numeroRevisao)
	{
		return _regras.FirstOrDefault(r => r.NumeroRevisao == numeroRevisao);
	}

	public List<RegraRevisao> ListarTodas()
	{
		return new List<RegraRevisao>(_regras);
	}
}
