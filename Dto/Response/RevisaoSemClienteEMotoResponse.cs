using Motos.Enums;

namespace Motos.Dto.Response;

public record class RevisaoSemClienteEMotoResponse(
    int Id,
    int Numero,
    StatusRevisao Status,
    DateOnly? DataAgendada,
    DateOnly? DataExecucao,
    double ValorTotal,
    ConcessionariaResponse? ConcessionariaResponsavel,
    RevisaoItemResponse[] Itens
);