namespace Motos.Dto.Response;

public record class RevisaoResponse(
    int Id,
    int Numero,
    ClienteResponse Cliente,
    MotoResponse Moto,
    bool Executada,
    int KmMaximo,
    double KmTolerancia,
    int TempoMaximo,
    double TempoTolerancia,
    int? KmAtual,
    DateTime DataRevisao,
    RevisaoItemResponse[] Itens
);