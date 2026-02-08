using System.Text.Json;
using Motos.Models;

namespace Motos.Services;

public static class ChecklistTemplateService
{
    private static List<ChecklistTemplate> Templates = new();

    public static void Inicializar()
    {
        try
        {
            string caminho = Path.Combine(Directory.GetCurrentDirectory(), "Utils", "checklists-template.json");
            if (!File.Exists(caminho))
                throw new FileNotFoundException($"Arquivo de templates não encontrado: {caminho}");

            string json = File.ReadAllText(caminho);
            var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var carregados = JsonSerializer.Deserialize<List<ChecklistTemplate>>(json, opcoes);

            Templates = carregados ?? new();
            Console.WriteLine($"✓ {Templates.Count} templates de checklist carregados com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Erro ao carregar templates de checklist: {ex.Message}");
            throw;
        }
    }

    public static ChecklistTemplate? ObterPorId(int id) => Templates.FirstOrDefault(t => t.Id == id);

    public static List<ChecklistTemplate> ListarTodos() => new List<ChecklistTemplate>(Templates);

    public static List<ChecklistTemplate> ListarPorNumeroRevisao(int numero) => Templates.Where(t => t.NumeroRevisao == numero).ToList();

    public static List<ChecklistTemplate> ListarPorModelo(string modelo) => Templates.Where(t => t.Modelos.Contains(modelo)).ToList();
}
