using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record ConcessionariaRequest (
    [Required(ErrorMessage = "O Nome é obrigatório.")]
    string Nome,
    [Required(ErrorMessage = "O Telefone é obrigatório.")]
    string Telefone,
    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    string Cnpj
);