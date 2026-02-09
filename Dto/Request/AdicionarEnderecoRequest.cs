using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record AdicionarEnderecoRequest(
    [Required(ErrorMessage = "A Rua é obrigatória.")]
    string Rua,
    [Required(ErrorMessage = "O número é obrigatório.")]
    int Numero,
    string? Complemento,
    [Required(ErrorMessage = "O bairro é obrigatório.")]
    string Bairro,
    [Required(ErrorMessage = "A cidade é obrigatória.")]
    string Cidade,
    [Required(ErrorMessage = "O estado é obrigatório.")]
    string Estado,
    [Required(ErrorMessage = "O CEP é obrigatório.")]
    string Cep
);