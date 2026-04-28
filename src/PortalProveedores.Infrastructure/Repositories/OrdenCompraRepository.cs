using Microsoft.EntityFrameworkCore;
using PortalProveedores.Domain.Entities;
using PortalProveedores.Domain.Interfaces;
using PortalProveedores.Infrastructure.Persistence;

namespace PortalProveedores.Infrastructure.Repositories;

public class OrdenCompraRepository(AppDbContext db) : IOrdenCompraRepository
{
    public async Task<IEnumerable<OrdenCompra>> GetByProveedorNitAsync(string nit) =>
        await db.OrdenesCompra
            .Where(o => o.ProveedorNit == nit)
            .ToListAsync();

    public async Task<OrdenCompra?> GetByIdAsync(int id) =>
        await db.OrdenesCompra.FindAsync(id);

    public async Task UpsertAsync(IEnumerable<OrdenCompra> ordenes)
    {
        foreach (var orden in ordenes)
        {
            var existing = await db.OrdenesCompra
                .FirstOrDefaultAsync(o => o.NumeroOC == orden.NumeroOC);
            if (existing is null)
                db.OrdenesCompra.Add(orden);
            else
            {
                existing.CantidadPendiente = orden.CantidadPendiente;
                existing.FechaEntrega = orden.FechaEntrega;
                existing.Urgente = orden.Urgente;
                existing.SyncFecha = DateTime.UtcNow;
            }
        }
        await db.SaveChangesAsync();
    }
}
