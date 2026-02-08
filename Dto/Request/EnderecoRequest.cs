using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record EnderecoRequest(
    [Required(ErrorMessage = "O Cep é obrigatório")]
    string Cep,
    [Required(ErrorMessage = "A Rua é obrigatória")]
    string Rua,
    [Required(ErrorMessage = "O Número é obrigatório")]
    int Numero,
    [Required(ErrorMessage = "O Bairro é obrigatório")]
    string Bairro,
    string? Complemento
);