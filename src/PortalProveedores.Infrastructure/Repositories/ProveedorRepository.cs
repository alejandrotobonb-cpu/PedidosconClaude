using Microsoft.EntityFrameworkCore;
using PortalProveedores.Domain.Entities;
using PortalProveedores.Domain.Interfaces;
using PortalProveedores.Infrastructure.Persistence;

namespace PortalProveedores.Infrastructure.Repositories;

public class ProveedorRepository(AppDbContext db) : IProveedorRepository
{
    public async Task<Proveedor?> GetByAzureAdObjectIdAsync(string objectId) =>
        await db.Proveedores.FirstOrDefaultAsync(p => p.AzureAdObjectId == objectId);

    public async Task<Proveedor?> GetByNitAsync(string nit) =>
        await db.Proveedores.FirstOrDefaultAsync(p => p.Nit == nit);
}
