namespace Motos.Models;

public class Endereco
{
    public int Id { get; set; }
    public required string Cep { get; set; }
    public required string Rua { get; set; } 
    public required int Numero { get; set; }
    public required string Bairro { get; set; }
    public required string Cidade { get; set; }
    public required string Estado { get; set; }
    public string? Complemento { get; set; }
}