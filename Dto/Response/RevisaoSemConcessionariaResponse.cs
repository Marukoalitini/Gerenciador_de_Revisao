using Motos.Enums;

namespace Motos.Dto.Response;

public record RevisaoSemConcessionariaResponse(
    int Id,
    int Numero,
    StatusRevisao Status,
    ClienteResponse Cliente,
    MotoResponse Moto,
    int? KmAtual,
    DateOnly? DataAgendada,
    DateOnly? DataExecucao,
    double ValorTotal,
    string? NotaDeServico,
    ConcessionariaResponse? ConcessionariaResponsavel,
    RevisaoItemResponse[] Itens
);