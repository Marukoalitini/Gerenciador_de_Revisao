using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motos.Models;

namespace Motos.Data.Configurations;

public class ConcessionariaConfiguration : IEntityTypeConfiguration<Concessionaria>
{
    public void Configure(EntityTypeBuilder<Concessionaria> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomeConcessionaria)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(c => c.Cnpj)
            .IsUnique();

        builder.Property(c => c.Telefone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Senha)
            .IsRequired()
            .HasMaxLength(255);
    }
}
