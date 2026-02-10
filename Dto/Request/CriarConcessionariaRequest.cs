using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record CriarConcessionariaRequest (
    [Required(ErrorMessage = "O Nome é obrigatório.")]
    string NomeConcessionaria,
    [Required(ErrorMessage = "O Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    string Email,
    [Required(ErrorMessage = "A Senha é obrigatória.")]
    string Senha,
    [Required(ErrorMessage = "O Telefone é obrigatório.")]
    string Telefone,
    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    string Cnpj
);