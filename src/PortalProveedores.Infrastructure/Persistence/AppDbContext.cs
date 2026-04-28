using Microsoft.EntityFrameworkCore;
using PortalProveedores.Domain.Entities;

namespace PortalProveedores.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Adjunto> Adjuntos => Set<Adjunto>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<OrdenCompra>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.NumeroOC).HasMaxLength(50).IsRequired();
            e.Property(o => o.ProveedorNit).HasMaxLength(20).IsRequired();
            e.Property(o => o.Articulo).HasMaxLength(200).IsRequired();
            e.Property(o => o.CodigoArticulo).HasMaxLength(50).IsRequired();
            e.Property(o => o.Finca).HasMaxLength(150).IsRequired();
            e.Property(o => o.Descripcion).HasMaxLength(500);
            e.Property(o => o.ObsCompras).HasMaxLength(1000);
            e.Property(o => o.UnidadMedida).HasMaxLength(20);
            e.Property(o => o.CantidadPedida).HasColumnType("decimal(10,2)");
            e.Property(o => o.CantidadPendiente).HasColumnType("decimal(10,2)");
            e.HasIndex(o => o.ProveedorNit);
            e.HasIndex(o => o.NumeroOC).IsUnique();
            e.Ignore(o => o.DiasVencimiento);
        });

        model.Entity<Comentario>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Texto).HasMaxLength(2000).IsRequired();
            e.Property(c => c.GuiaTransporte).HasMaxLength(100);
            e.Property(c => c.ProveedorNit).HasMaxLength(20).IsRequired();
            e.Property(c => c.UsuarioId).HasMaxLength(100).IsRequired();
            e.HasOne(c => c.OrdenCompra)
             .WithMany(o => o.Comentarios)
             .HasForeignKey(c => c.OrdenCompraId);
            e.HasMany(c => c.Adjuntos)
             .WithOne(a => a.Comentario)
             .HasForeignKey(a => a.ComentarioId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Proveedor>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Nit).HasMaxLength(20).IsRequired();
            e.Property(p => p.RazonSocial).HasMaxLength(200).IsRequired();
            e.Property(p => p.EmailContacto).HasMaxLength(150);
            e.Property(p => p.AzureAdObjectId).HasMaxLength(100);
            e.Property(p => p.CompradorEmail).HasMaxLength(150);
            e.HasIndex(p => p.Nit).IsUnique();
            e.HasIndex(p => p.AzureAdObjectId);
        });

        model.Entity<Adjunto>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.NombreArchivo).HasMaxLength(255).IsRequired();
            e.Property(a => a.TipoMime).HasMaxLength(100).IsRequired();
            e.Property(a => a.BlobUri).HasMaxLength(500).IsRequired();
        });
    }
}
