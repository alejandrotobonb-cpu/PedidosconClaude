using Microsoft.EntityFrameworkCore;
using Portal.Domain.Entities;

namespace Portal.Domain.Interfaces;

public interface IPortalDbContext
{
    DbSet<Proveedor> Proveedores { get; }
    DbSet<OrdenCompra> OrdenesCompra { get; }
    DbSet<Comentario> Comentarios { get; }
    DbSet<Adjunto> Adjuntos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
