using Motos.Enums;

namespace Motos.Dto.Response;

public record RevisaoSemConcessionariaResponse(
    int Id,
    int Numero,
    StatusRevisao Status,
    ClienteResponse Cliente,
    MotoResponse Moto,
    DateOnly? DataAgendada,
    DateOnly? DataExecucao,
    double ValorTotal,
    ConcessionariaResponse? ConcessionariaResponsavel,
    RevisaoItemResponse[] Itens
);