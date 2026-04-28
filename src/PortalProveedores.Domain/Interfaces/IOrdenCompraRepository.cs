using PortalProveedores.Domain.Entities;

namespace PortalProveedores.Domain.Interfaces;

public interface IOrdenCompraRepository
{
    // Fetches all pending orders for a supplier — filtering is done in-memory at this scale.
    // Move predicate into DB query if row count grows beyond ~10k.
    Task<IEnumerable<OrdenCompra>> GetByProveedorNitAsync(string nit);
    Task<OrdenCompra?> GetByIdAsync(int id);
    Task UpsertAsync(IEnumerable<OrdenCompra> ordenes);
}
