using Microsoft.EntityFrameworkCore;
using Portal.Domain.Entities;
using Portal.Domain.Interfaces;
using Portal.Infrastructure.Persistence.Configurations;

namespace Portal.Infrastructure.Persistence;

public class PortalDbContext(DbContextOptions<PortalDbContext> options) : DbContext(options), IPortalDbContext
{
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<Adjunto> Adjuntos => Set<Adjunto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProveedorConfiguration());
        modelBuilder.ApplyConfiguration(new OrdenCompraConfiguration());
        modelBuilder.ApplyConfiguration(new ComentarioConfiguration());
        modelBuilder.ApplyConfiguration(new AdjuntoConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
