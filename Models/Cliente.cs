namespace Motos.Models;

public class Cliente
{
    int Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Senha { get; set; }
    public Endereco? Endereco { get; set; }
    public required string Telefone { get; set; }
    public required string Celular { get; set; }
    public required string Cpf { get; set; }
}