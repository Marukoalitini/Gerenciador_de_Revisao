namespace Motos.Models;

public class ChecklistTemplate
{
    public int Id { get; set; }
    public int NumeroRevisao { get; set; }

    // Por enquanto armazenamos modelos como strings (vêm do JSON).
    // Posteriormente podemos mapear para um enum ModeloMoto.
    public List<string> Modelos { get; set; } = new();

    public List<ItemTemplate> Itens { get; set; } = new();

    // Relação Many-to-Many com Concessionaria
    public List<Concessionaria> Concessionarias { get; set; } = new();

    public void AdicionarItem(ItemTemplate item)
    {
        Itens.Add(item);
    }

    public void RemoverItem(int ordem)
    {
        var item = Itens.FirstOrDefault(i => i.Ordem == ordem);
        if (item != null) Itens.Remove(item);
    }

    public List<RevisaoItem> GerarItensParaRevisao()
    {
        return Itens.Select(i => i.ParaRevisaoItem()).ToList();
    }
}
