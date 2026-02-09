using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record AtualizarConcessionariaRequest(
    string? Nome,
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    string? Email,
    string? Telefone
);