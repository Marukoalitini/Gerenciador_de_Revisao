using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record MotoRequest(
    [Required(ErrorMessage = "A cor é obrigatória.")]
    string Cor,
    [Required(ErrorMessage = "O número do chassi é obrigatório.")]
    string NumeroChassi,
    [Required(ErrorMessage = "A placa é obrigatória.")]
    string Placa,
    [Required(ErrorMessage = "A data de venda é obrigatória.")]
    DateTime DataDeVenda,
    [Required(ErrorMessage = "A nota fiscal é obrigatória.")]
    string NotaFiscal,
    [Required(ErrorMessage = "A série é obrigatória.")]
    int Serie,
    string? ImgDecalqueChassi
);