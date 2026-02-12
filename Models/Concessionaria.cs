namespace Motos.Models;

public class Concessionaria
{
    public int Id { get; set; }
    public required string NomeConcessionaria { get; set; }
    public required string Email { get; set; }
    public required string Senha { get; set; }
    public required string Telefone { get; set; }
    public List<Endereco> Enderecos { get; set; } = [];
    public required string Cnpj { get; set; }
    public List<Revisao> Revisoes { get; set; } = [];
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? DeletadoEm { get; set; }
    public bool Ativo { get; set; } = true;
}