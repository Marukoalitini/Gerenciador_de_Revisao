using Motos.Enums;

namespace Motos.Dto.Response;

public record RevisaoItemResponse(
    int Id,
    TipoItemRevisao Tipo,
    string Descricao,
    string? Grupo,
    int Ordem,
    double? Valor,
    double? Quantidade,
    bool Realizado
);