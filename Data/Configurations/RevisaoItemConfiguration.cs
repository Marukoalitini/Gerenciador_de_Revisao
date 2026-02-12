using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motos.Models;

namespace Motos.Data.Configurations;

public class RevisaoItemConfiguration : IEntityTypeConfiguration<RevisaoItem>
{
    public void Configure(EntityTypeBuilder<RevisaoItem> builder)
    {
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Tipo)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ri => ri.Descricao)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ri => ri.Grupo)
            .HasMaxLength(50);
    }
}
