using System.ComponentModel.DataAnnotations;

namespace Motos.Dto.Request;

public record SolicitarAgendamentoRequest(
    [Required(ErrorMessage = "A data desejada é obrigatória.")]
    DateOnly DataDesejada,
    
    [Required(ErrorMessage = "A concessionária é obrigatória.")]
    int ConcessionariaId
);