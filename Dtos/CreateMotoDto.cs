using System.ComponentModel.DataAnnotations;

namespace Motos.Dtos;

public class CreateMotoDto
{
    [Required]
    public string Marca { get; set; } = string.Empty;
    
    [Required]
    public string Modelo { get; set; } = string.Empty;
    
    [Required]
    public int Ano { get; set; }
}
