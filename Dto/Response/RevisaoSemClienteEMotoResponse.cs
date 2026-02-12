using Motos.Enums;

namespace Motos.Dto.Response;

public record class RevisaoSemClienteEMotoResponse(
    int Id,
    int Numero,
    StatusRevisao Status,
    int? KmAtual,
    DateOnly? DataAgendada,
    DateOnly? DataExecucao,
    double ValorTotal,
    string? NotaDeServico,
    ConcessionariaResponse? ConcessionariaResponsavel,
    RevisaoItemResponse[] Itens
);