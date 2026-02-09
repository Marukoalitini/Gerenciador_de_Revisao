using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record LoginRequest(
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória.")]
    string Senha
);