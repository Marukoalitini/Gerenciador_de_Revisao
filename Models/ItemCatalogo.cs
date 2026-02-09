using Motos.Enums;

namespace Motos.Models;

public class ItemCatalogo
{
	public int Id { get; set; }
	public TipoItemRevisao Tipo { get; set; }
	public string Descricao { get; set; } = null!;
	public decimal? Preco { get; set; }
}
