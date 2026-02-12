using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motos.Models;

namespace Motos.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomeCliente)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(c => c.Cpf)
            .IsUnique();

        builder.Property(c => c.Telefone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Celular)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Senha)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(c => c.Motos)
            .WithOne(m => m.Cliente)
            .HasForeignKey(m => m.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
