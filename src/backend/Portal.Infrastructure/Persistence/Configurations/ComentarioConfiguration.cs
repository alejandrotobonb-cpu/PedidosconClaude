using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Domain.Entities;

namespace Portal.Infrastructure.Persistence.Configurations;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ProveedorNit).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Texto).IsRequired();
        builder.Property(c => c.NumeroGuia).HasMaxLength(100);
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(c => c.OrdenCompra)
            .WithMany(o => o.Comentarios)
            .HasForeignKey(c => c.OrdenCompraId);

        builder.HasOne(c => c.Proveedor)
            .WithMany(p => p.Comentarios)
            .HasForeignKey(c => c.ProveedorNit)
            .HasPrincipalKey(p => p.Nit);
    }
}
