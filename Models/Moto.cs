namespace Motos.Models;

public class Moto
{
    public int Id { get; set; }
    //public required Enum ModeloMoto { get; set; }
    public required string Cor { get; set; }
    public required string NumeroChassi { get; set; }
    public required DateTime DataDeVenda { get; set; }
    public required string NotaFiscal { get; set; }
    public required int Serie { get; set; }
    public string ImgDecalqueChassi = string.Empty;
    public List<Revisao> Revisoes = [];
}
