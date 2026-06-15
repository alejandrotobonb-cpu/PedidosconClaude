using Portal.Domain.Entities;

namespace Portal.Domain.Interfaces;

public interface ISagClient
{
    Task<IReadOnlyList<OrdenCompra>> ObtenerOrdenesPendientesAsync(
        string nit, CancellationToken ct = default);
}
