using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motos.Models;

namespace Motos.Data.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Cep)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(e => e.Rua)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Bairro)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Cidade)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(e => e.Complemento)
            .HasMaxLength(100);
    }
}
