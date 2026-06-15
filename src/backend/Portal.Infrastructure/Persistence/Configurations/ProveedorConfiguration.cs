using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Domain.Entities;

namespace Portal.Infrastructure.Persistence.Configurations;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("Proveedores");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nit).HasMaxLength(20).IsRequired();
        builder.HasIndex(p => p.Nit).IsUnique();
        builder.Property(p => p.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(p => p.EmailSac).HasMaxLength(100).IsRequired();
        builder.Property(p => p.CompradorEmail).HasMaxLength(100).IsRequired();
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
