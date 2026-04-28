using PortalProveedores.Application.DTOs;

namespace PortalProveedores.Application.Interfaces;

public interface IOrdenesPorProveedor
{
    Task<IEnumerable<OrdenCompraDto>> ExecuteAsync(string nit);
}
