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
    public DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
    public DbSet<ItemTemplate> ItemTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Many-to-many between Concessionaria and ChecklistTemplate
        modelBuilder.Entity<Concessionaria>()
            .HasMany(c => c.ChecklistTemplates)
            .WithMany(t => t.Concessionarias)
            .UsingEntity("ConcessionariaChecklistTemplate");

        // Moto unique constraints
        modelBuilder.Entity<Moto>()
            .HasIndex(m => m.Placa)
            .IsUnique(false);

        modelBuilder.Entity<Moto>()
            .HasIndex(m => m.NumeroChassi)
            .IsUnique(false);

        // Cliente -> Motos (1 - many)
        modelBuilder.Entity<Cliente>()
            .HasMany(c => c.Motos)
            .WithOne(m => m.Cliente)
            .HasForeignKey(m => m.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);

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
            .HasOne(r => r.ConcessinariaResposavel)
            .WithMany(c => c.Revisoes)
            .HasForeignKey(r => r.ConcessionariaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
