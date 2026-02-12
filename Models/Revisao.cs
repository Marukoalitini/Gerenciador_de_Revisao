using Motos.Enums;

namespace Motos.Models;

public class Revisao
{
    public int Id { get; set; }
    public required int Numero { get; set; }
    public int ClienteId { get; set; }
    public required Cliente Cliente { get; set; }
    public int MotoId { get; set; }
    public required Moto Moto { get; set; }
    public StatusRevisao Status { get; set; } = StatusRevisao.Pendente;

    public int? KmAtual { get; set; }
    public DateOnly? DataAgendada { get; set; }
    public DateOnly? DataExecucao { get; set; }
    public List<RevisaoItem> Itens { get; set; } = [];
    public double ValorTotal { get; set; } = 0.0;
    public string? NotaDeServico { get; set; }
    public int? ConcessionariaResponsavelId { get; set; }
    public Concessionaria? ConcessionariaResponsavel { get; set; }
    
}