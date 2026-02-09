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
    }
}
