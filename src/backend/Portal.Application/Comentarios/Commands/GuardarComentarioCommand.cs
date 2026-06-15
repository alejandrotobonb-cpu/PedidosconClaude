using MediatR;
using Microsoft.EntityFrameworkCore;
using Portal.Domain.Entities;
using Portal.Domain.Interfaces;

namespace Portal.Application.Comentarios.Commands;

public record GuardarComentarioCommand(
    int OrdenCompraId,
    string ProveedorNit,
    string Texto,
    DateOnly? FechaCompromiso,
    string? NumeroGuia) : IRequest<int>;

public class GuardarComentarioHandler(IPortalDbContext db, IEmailService emailService)
    : IRequestHandler<GuardarComentarioCommand, int>
{
    public async Task<int> Handle(
        GuardarComentarioCommand request, CancellationToken cancellationToken)
    {
        var orden = await db.OrdenesCompra
            .Include(o => o.Proveedor)
            .FirstOrDefaultAsync(o =>
                o.Id == request.OrdenCompraId &&
                o.ProveedorNit == request.ProveedorNit, cancellationToken)
            ?? throw new InvalidOperationException("Orden no encontrada o sin acceso.");

        var comentario = new Comentario
        {
            OrdenCompraId = request.OrdenCompraId,
            ProveedorNit = request.ProveedorNit,
            Texto = request.Texto,
            FechaCompromiso = request.FechaCompromiso,
            NumeroGuia = request.NumeroGuia
        };

        db.Comentarios.Add(comentario);
        await db.SaveChangesAsync(cancellationToken);

        await emailService.EnviarNotificacionCompradorAsync(
            compradorEmail: orden.Proveedor.CompradorEmail,
            proveedorNombre: orden.Proveedor.Nombre,
            numeroOc: orden.NumeroOc,
            fuenteFinca: orden.FuenteFinca ?? "",
            descripcionArticulo: orden.Descripcion ?? "",
            textoComentario: request.Texto,
            fechaCompromiso: request.FechaCompromiso,
            numeroGuia: request.NumeroGuia,
            nombresAdjuntos: [],
            cancellationToken: cancellationToken);

        await MarkNotificadoAsync(comentario.Id, cancellationToken);

        return comentario.Id;
    }

    private async Task MarkNotificadoAsync(int comentarioId, CancellationToken ct)
    {
        var c = await db.Comentarios.FindAsync([comentarioId], ct);
        if (c is not null) { c.Notificado = true; await db.SaveChangesAsync(ct); }
    }
}
