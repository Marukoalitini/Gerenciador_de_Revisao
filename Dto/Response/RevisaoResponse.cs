using Motos.Enums;

namespace Motos.Dto.Response;

public record class RevisaoResponse(
    int Id,
    int Numero,
    ClienteResponse Cliente,
    MotoResponse Moto,
    StatusRevisao Status,
    int? KmAtual,
    DateTime? DataAgendada,
    DateTime? DataExecucao,
    double ValorTotal,
    string? NotaDeServico,
    ConcessionariaResponse? ConcessionariaResponsavel,
    RevisaoItemResponse[] Itens
);