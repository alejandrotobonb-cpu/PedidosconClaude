using PortalProveedores.Domain.Entities;

namespace PortalProveedores.Domain.Interfaces;

public interface IProveedorRepository
{
    Task<Proveedor?> GetByAzureAdObjectIdAsync(string objectId);
    Task<Proveedor?> GetByNitAsync(string nit);
}
