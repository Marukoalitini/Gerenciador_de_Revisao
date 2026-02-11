using Motos.Enums;

namespace Motos.Models;

public class Moto
{
    public int Id { get; set; }
    public required ModeloMoto ModeloMoto { get; set; }
    public required string Cor { get; set; }
    public required string NumeroChassi { get; set; }
    public required string Placa { get; set; }
    public required DateTime DataDeVenda { get; set; }
    public required string NotaFiscal { get; set; }
    public required int Serie { get; set; }
    public string ImgDecalqueChassi { get; set; } = string.Empty;
    public List<Revisao> Revisoes { get; set; } = [];

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime? DeletadoEm { get; set; }
}
