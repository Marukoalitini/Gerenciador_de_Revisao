namespace Motos.Models;

public class Cliente
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Senha { get; set; }
    public Endereco? Endereco { get; set; }
    public List<Moto> Motos { get; set; } = new();
    public required string Telefone { get; set; }
    public required string Celular { get; set; }
    public required string Cpf { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? DeletadoEm { get; set; }
    public bool Ativo { get; set; } = true;
    public List<Moto> Motos { get; set; } = [];
}