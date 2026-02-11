using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record AgendarRevisaoRequest(
    [Required(ErrorMessage = "A data do agendamento é obrigatória.")]
    DateTime DataAgendamento,
    
    [Required(ErrorMessage = "A concessionária responsável é obrigatória.")]
    int ConcessionariaId
);