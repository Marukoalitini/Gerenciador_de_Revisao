using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record CriarClienteRequest
(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string NomeCliente,
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
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