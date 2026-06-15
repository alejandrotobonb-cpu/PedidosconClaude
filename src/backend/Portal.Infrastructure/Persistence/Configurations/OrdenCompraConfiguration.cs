using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Domain.Entities;

namespace Portal.Infrastructure.Persistence.Configurations;

public class OrdenCompraConfiguration : IEntityTypeConfiguration<OrdenCompra>
{
    public void Configure(EntityTypeBuilder<OrdenCompra> builder)
    {
        builder.ToTable("OrdenesCompra");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.NumeroOc).HasMaxLength(20).IsRequired();
        builder.Property(o => o.ProveedorNit).HasMaxLength(20).IsRequired();
        builder.Property(o => o.FuenteFinca).HasMaxLength(100);
        builder.Property(o => o.CodigoArt).HasMaxLength(50);
        builder.Property(o => o.CantidadPedida).HasColumnType("decimal(10,2)");
        builder.Property(o => o.CantidadPend).HasColumnType("decimal(10,2)");
        builder.Property(o => o.SincronizadoEn).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(o => new { o.NumeroOc, o.CodigoArt }).IsUnique();
        builder.Ignore(o => o.DiasVencimiento);

        builder.HasOne(o => o.Proveedor)
            .WithMany(p => p.OrdenesCompra)
            .HasForeignKey(o => o.ProveedorNit)
            .HasPrincipalKey(p => p.Nit);
    }
}
