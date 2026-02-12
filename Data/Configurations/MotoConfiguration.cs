using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motos.Models;

namespace Motos.Data.Configurations;

public class MotoConfiguration : IEntityTypeConfiguration<Moto>
{
    public void Configure(EntityTypeBuilder<Moto> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Cor)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(m => m.NumeroChassi)
            .IsRequired()
            .HasMaxLength(17);

        builder.HasIndex(m => m.NumeroChassi)
            .IsUnique();

        builder.Property(m => m.Placa)
            .IsRequired()
            .HasMaxLength(7);

        builder.HasIndex(m => m.Placa)
            .IsUnique();

        builder.Property(m => m.NotaFiscal)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(m => m.ImgDecalqueChassi)
            .HasMaxLength(500);

        builder.Property(m => m.ModeloMoto)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
