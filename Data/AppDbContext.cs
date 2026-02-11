using Microsoft.EntityFrameworkCore;
using Motos.Models;

namespace Motos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Moto> Motos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Revisao> Revisoes { get; set; }
    public DbSet<Concessionaria> Concessionarias { get; set; }
    public DbSet<RevisaoItem> RevisaoItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Moto>()
            .HasOne(m => m.Cliente)
            .WithMany(c => c.Motos)
            .HasForeignKey(m => m.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Moto unique constraints
        modelBuilder.Entity<Moto>()
            .HasIndex(m => m.Placa)
            .IsUnique(true);

        modelBuilder.Entity<Moto>()
            .HasIndex(m => m.NumeroChassi)
            .IsUnique(true);

        // Revisao status enum as string
        modelBuilder.Entity<Revisao>()
            .Property(r => r.Status)
            .HasConversion<string>();

        // Revisao relationships
        modelBuilder.Entity<Revisao>()
            .HasOne(r => r.Cliente)
            .WithMany()
            .HasForeignKey(r => r.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Revisao>()
            .HasOne(r => r.Moto)
            .WithMany(m => m.Revisoes)
            .HasForeignKey(r => r.MotoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Revisao>()
            .HasOne(r => r.ConcessionariaResponsavel)
            .WithMany(c => c.Revisoes)
            .HasForeignKey(r => r.ConcessionariaResponsavelId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
