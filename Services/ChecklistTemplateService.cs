using System.Text.Json;
using Motos.Models;

namespace Motos.Services;

public class ChecklistTemplateService
{
    private List<ChecklistTemplate> _templates = [];

    public ChecklistTemplateService()
    {
        Inicializar();
    }

    public void Inicializar()
    {
        try
        {
            string caminho = Path.Combine(Directory.GetCurrentDirectory(), "Utils", "checklists-template.json");
            if (!File.Exists(caminho))
                throw new FileNotFoundException($"Arquivo de templates não encontrado: {caminho}");

            string json = File.ReadAllText(caminho);
            var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var carregados = JsonSerializer.Deserialize<List<ChecklistTemplate>>(json, opcoes);

            _templates = carregados ?? new();
            Console.WriteLine($"✓ {_templates.Count} templates de checklist carregados com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Erro ao carregar templates de checklist: {ex.Message}");
            throw;
        }
    }

    public ChecklistTemplate? ObterPorId(int id) => _templates.FirstOrDefault(t => t.Id == id);

    public List<ChecklistTemplate> ListarTodos() => new List<ChecklistTemplate>(_templates);

    public List<ChecklistTemplate> ListarPorNumeroRevisao(int numero) => _templates.Where(t => t.NumeroRevisao == numero).ToList();

    public List<ChecklistTemplate> ListarPorModelo(string modelo) => _templates.Where(t => t.Modelos.Contains(modelo)).ToList();
}
