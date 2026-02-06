namespace Motos.Models;

public class Revisao
{
    int Id { get; set; } 
    public required int Numero { get; set; }
    public required Cliente Cliente { get; set; }
    public required Moto Moto { get; set; }
    public bool Executada { get; set; } = false;
    public required int KmMaximo { get; set; }
    public double KmTolerancia { get; set; } = 10.0;
    public required int TempoMaximo { get; set; }
    public double TempoTolerancia { get; set; } = 10.0;
    public int? KmAtual { get; set; }
    public required DateTime DataRevisao { get; set; }
    public List<RevisaoItem> Itens { get; set; } = [];
    public double ValorTotal { get; set; } = 0.0;
    public string? NotaDeServico { get; set; }
    public Concessionaria? ConcessinariaResposavel { get; set; }
}