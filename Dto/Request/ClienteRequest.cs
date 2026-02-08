using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record ClienteRequest
(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Nome,
    [Required(ErrorMessage = "O email é obrigatório.")]
    string Email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    string Senha,
    [Required(ErrorMessage = "O telefone é obrigatório.")]
    string Telefone,
    [Required(ErrorMessage = "O celular é obrigatório.")]
    string Celular,
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    string Cpf
);