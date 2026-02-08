namespace Motos.Models;

public class Concessionaria
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Telefone { get; set; }
    public List<Endereco> Enderecos { get; set; } = [];
    public required string Cnpj { get; set; }
    public List<Revisao> Revisoes { get; set; } = [];
}