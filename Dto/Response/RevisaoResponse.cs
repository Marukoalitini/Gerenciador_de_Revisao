using Motos.Enums;

namespace Motos.Dto.Response;

public record class RevisaoResponse(
    int Id,
    int Numero,
    ClienteResponse Cliente,
    MotoResponse Moto,
    StatusRevisao Status,
    int KmMaximo,
    double KmTolerancia,
    int TempoMaximo,
    int TempoTolerancia,
    int? KmAtual,
    DateTime DataRevisao,
    double ValorTotal,
    string? NotaDeServico,
    ConcessionariaResponse? ConcessionariaResponsavel,
    RevisaoItemResponse[] Itens
);