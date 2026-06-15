using MediatR;
using Microsoft.EntityFrameworkCore;
using Portal.Domain.Interfaces;

namespace Portal.Application.OrdenesPendientes.Queries;

public record GetOrdenesPendientesQuery(string ProveedorNit) : IRequest<List<OrdenPendienteDto>>;

public record OrdenPendienteDto(
    int Id,
    string NumeroOc,
    string? FuenteFinca,
    string? CodigoArt,
    string? Descripcion,
    DateOnly? FechaPedido,
    DateOnly? FechaEntrega,
    decimal? CantidadPedida,
    decimal? CantidadPend,
    string? ObsCompras,
    bool Urgente,
    int DiasVencimiento,
    string? UltimoComentario,
    DateOnly? FechaCompromiso);

public class GetOrdenesPendientesHandler(IPortalDbContext db)
    : IRequestHandler<GetOrdenesPendientesQuery, List<OrdenPendienteDto>>
{
    public async Task<List<OrdenPendienteDto>> Handle(
        GetOrdenesPendientesQuery request, CancellationToken cancellationToken)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        return await db.OrdenesCompra
            .Where(o => o.ProveedorNit == request.ProveedorNit && o.CantidadPend > 0)
            .OrderBy(o => o.FechaEntrega)
            .Select(o => new OrdenPendienteDto(
                o.Id,
                o.NumeroOc,
                o.FuenteFinca,
                o.CodigoArt,
                o.Descripcion,
                o.FechaPedido,
                o.FechaEntrega,
                o.CantidadPedida,
                o.CantidadPend,
                o.ObsCompras,
                o.Urgente,
                o.FechaEntrega.HasValue
                    ? (int)(o.FechaEntrega.Value.DayNumber - hoy.DayNumber)
                    : 0,
                o.Comentarios.OrderByDescending(c => c.CreatedAt)
                    .Select(c => c.Texto).FirstOrDefault(),
                o.Comentarios.OrderByDescending(c => c.CreatedAt)
                    .Select(c => c.FechaCompromiso).FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }
}
