namespace Motos.Dto.Response;

public record InfoPrevisaoRevisao(
    int Numero,
    DateTime DataPrevista,
    bool Atrasada,
    int DiasRestantes
);