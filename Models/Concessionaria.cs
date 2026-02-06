namespace Motos.Models;

public class Concessionaria
{
    int Id { get; set; }
    public required string Nome;
    public required string Telefone;
    public List<Endereco> Enderecos = [];
    public required string Cnpj;
    public List<Revisao> Revisoes = [];
}