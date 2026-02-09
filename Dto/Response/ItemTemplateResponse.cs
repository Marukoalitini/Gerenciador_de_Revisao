namespace Motos.Dto.Response;

public record class ItemTemplateResponse(
    int Id,
    int ItemCatalogoId,
    string? Grupo,
    int Ordem,
    decimal? ValorSugerido
);
