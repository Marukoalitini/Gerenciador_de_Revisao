using Motos.Enums;

namespace Motos.Models;

public class Revisao
{
    public int Id { get; set; }
    public required int Numero { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int MotoId { get; set; }
    public Moto? Moto { get; set; }
    public StatusRevisao Status { get; set; } = StatusRevisao.Pendente;
    public required int KmMaximo { get; set; }
    public double KmTolerancia { get; set; } = 10.0;
    public required int TempoMaximo { get; set; }
    public int TempoTolerancia { get; set; } = 15;
    public int? KmAtual { get; set; }
    public required DateTime DataRevisao { get; set; }
    public List<RevisaoItem> Itens { get; set; } = [];
    public double ValorTotal { get; set; } = 0.0;
    public string? NotaDeServico { get; set; }
    public int? ConcessionariaResponsavelId { get; set; }
    public Concessionaria? ConcessionariaResponsavel { get; set; }
    
}