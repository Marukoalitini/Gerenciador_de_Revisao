using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record RevisaoRequest(
    [Required(ErrorMessage = "O número da revisão é obrigatório.")]
    int Numero,
    [Required(ErrorMessage = "O ID do cliente é obrigatório.")]
    int ClienteId,
    [Required(ErrorMessage = "O ID da moto é obrigatório.")]
    int MotoId,
    int? KmAtual,
    [Required(ErrorMessage = "A data da revisão é obrigatória.")]
    DateOnly? DataAgendada,
    string? NotaDeServico,
    int? ConcessionariaResponsavelId);