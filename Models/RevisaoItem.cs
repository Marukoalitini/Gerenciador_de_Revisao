using Motos.Enums;

namespace Motos.Models;

public class RevisaoItem
{
    public int Id { get; set; }
    public required TipoItemRevisao Tipo { get; set; }
    public required string Descricao { get; set; }
    public string? Grupo { get; set; }
    public required int Ordem { get; set; }
    public required double? Valor { get; set; }
    public double? Quantidade { get; set; }
    public bool Realizado { get; set; } = true;
}