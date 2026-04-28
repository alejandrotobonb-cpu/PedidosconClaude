using PortalProveedores.Domain.Entities;

namespace PortalProveedores.Domain.Interfaces;

public interface IOrdenCompraRepository
{
    Task<IEnumerable<OrdenCompra>> GetByProveedorNitAsync(string nit);
    Task<OrdenCompra?> GetByIdAsync(int id);
    Task UpsertAsync(IEnumerable<OrdenCompra> ordenes);
}
