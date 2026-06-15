using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Domain.Entities;

namespace Portal.Infrastructure.Persistence.Configurations;

public class AdjuntoConfiguration : IEntityTypeConfiguration<Adjunto>
{
    public void Configure(EntityTypeBuilder<Adjunto> builder)
    {
        builder.ToTable("Adjuntos");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.NombreArchivo).HasMaxLength(255).IsRequired();
        builder.Property(a => a.TipoMime).HasMaxLength(100).IsRequired();
        builder.Property(a => a.BlobUri).HasMaxLength(500).IsRequired();
        builder.Property(a => a.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(a => a.Comentario)
            .WithMany(c => c.Adjuntos)
            .HasForeignKey(a => a.ComentarioId);
    }
}
