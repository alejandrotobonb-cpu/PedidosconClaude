using PortalProveedores.Application.DTOs;
using PortalProveedores.Application.Interfaces;
using PortalProveedores.Domain.Interfaces;

namespace PortalProveedores.Application.UseCases;

// Fix #6: implements IOrdenesPorProveedor so Controller depends on the interface, not the class
public class OrdenesPorProveedor(IOrdenCompraRepository ocRepo, IComentarioRepository comentarioRepo) : IOrdenesPorProveedor
{
    public async Task<IEnumerable<OrdenCompraDto>> ExecuteAsync(string nit)
    {
        var ordenes = await ocRepo.GetByProveedorNitAsync(nit);
        var comentarios = (await comentarioRepo.GetByProveedorNitAsync(nit))
            .GroupBy(c => c.OrdenCompraId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.FechaRegistro).First());

        return ordenes
            .OrderBy(o => o.DiasVencimiento)
            .Select(o =>
            {
                comentarios.TryGetValue(o.Id, out var ultimo);
                return new OrdenCompraDto(
                    o.Id, o.NumeroOC, o.Articulo, o.CodigoArticulo, o.Finca,
                    o.CantidadPendiente, o.UnidadMedida, o.FechaEntrega, o.DiasVencimiento, o.Urgente,
                    ultimo is null ? null : new ComentarioDto(
                        ultimo.Id, ultimo.Texto, ultimo.FechaCompromiso,
                        ultimo.GuiaTransporte, ultimo.FechaRegistro)
                );
            });
    }
}
