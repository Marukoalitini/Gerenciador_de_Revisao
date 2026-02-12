namespace Motos.Dto.Response;

public record InfoPrevisaoRevisao(
    int Numero,
    DateOnly DataPrevista,
    bool Atrasada,
    int DiasRestantes
);