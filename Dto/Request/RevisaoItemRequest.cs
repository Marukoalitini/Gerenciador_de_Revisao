using System.ComponentModel.DataAnnotations;
using Motos.Enums;

namespace Motos.Dto.Request;

public record RevisaoItemRequest(
    [Required(ErrorMessage = "O tipo do item é obrigatório.")]
    TipoItemRevisao Tipo,
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    string Descricao,
    string? Grupo,
    [Required(ErrorMessage = "A ordem é obrigatória.")]
    int Ordem,
    double? Valor,
    double? Quantidade,
    bool Realizado = true
);