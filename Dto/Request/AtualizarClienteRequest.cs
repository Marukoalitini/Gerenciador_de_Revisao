using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record AtualizarClienteRequest(
    string? Nome,
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    string? Email,
    string? Telefone,
    string? Celular
);