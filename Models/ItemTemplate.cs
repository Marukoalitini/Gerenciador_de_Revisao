namespace Motos.Models;

public class ItemTemplate
{
    public int Id { get; set; }
    public int ItemCatalogoId { get; set; }
    public string? Grupo { get; set; }
    public int Ordem { get; set; }
    public decimal? ValorSugerido { get; set; }

    // FK
    public int ChecklistTemplateId { get; set; }
    public ChecklistTemplate? ChecklistTemplate { get; set; }

    public RevisaoItem ParaRevisaoItem()
    {
        return new RevisaoItem
        {
            Tipo = Motos.Enums.TipoItemRevisao.Peca,
            Descricao = $"ItemCatalogoId:{ItemCatalogoId}",
            Grupo = Grupo,
            Ordem = Ordem,
            Valor = ValorSugerido.HasValue ? (double?)Convert.ToDouble(ValorSugerido.Value) : null,
            Quantidade = null,
            Realizado = false
        };
    }

    public RevisaoItem ParaRevisaoItem(ItemCatalogo itemCatalogo)
    {
        double? valor = null;
        if (ValorSugerido.HasValue) valor = Convert.ToDouble(ValorSugerido.Value);
        else if (itemCatalogo.Preco.HasValue) valor = Convert.ToDouble(itemCatalogo.Preco.Value);

        return new RevisaoItem
        {
            Tipo = itemCatalogo.Tipo,
            Descricao = itemCatalogo.Descricao,
            Grupo = Grupo,
            Ordem = Ordem,
            Valor = valor,
            Quantidade = null,
            Realizado = false
        };
    }
}
