using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motos.Models;

namespace Motos.Data.Configurations;

public class RevisaoConfiguration : IEntityTypeConfiguration<Revisao>
{
    public void Configure(EntityTypeBuilder<Revisao> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.NotaDeServico)
            .HasMaxLength(50);

        builder.HasOne(r => r.Cliente)
            .WithMany()
            .HasForeignKey(r => r.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Moto)
            .WithMany(m => m.Revisoes)
            .HasForeignKey(r => r.MotoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ConcessionariaResponsavel)
            .WithMany(c => c.Revisoes)
            .HasForeignKey(r => r.ConcessionariaResponsavelId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
