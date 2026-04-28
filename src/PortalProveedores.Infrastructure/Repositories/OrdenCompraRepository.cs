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

    // Fix #4: load all matching rows in one query, then update in memory — no N+1
    public async Task UpsertAsync(IEnumerable<OrdenCompra> ordenes)
    {
        var lista = ordenes.ToList();
        var numeros = lista.Select(o => o.NumeroOC).ToHashSet();

        var existentes = await db.OrdenesCompra
            .Where(o => numeros.Contains(o.NumeroOC))
            .ToDictionaryAsync(o => o.NumeroOC);

        foreach (var orden in lista)
        {
            if (existentes.TryGetValue(orden.NumeroOC, out var existing))
            {
                existing.CantidadPendiente = orden.CantidadPendiente;
                existing.FechaEntrega = orden.FechaEntrega;
                existing.Urgente = orden.Urgente;
                existing.SyncFecha = DateTime.UtcNow;
            }
            else
            {
                db.OrdenesCompra.Add(orden);
            }
        }

        await db.SaveChangesAsync();
    }
}
