using PortalProveedores.Application.DTOs;
using PortalProveedores.Application.Interfaces;
using PortalProveedores.Domain.Entities;
using PortalProveedores.Domain.Interfaces;

namespace PortalProveedores.Application.UseCases;

// Fix #3: builds all Comentario objects first, then inserts in one DB round-trip
// Fix #6: implements IGuardarComentario so Controller depends on the interface, not the class
public class GuardarComentario(IComentarioRepository repo) : IGuardarComentario
{
    public async Task<IEnumerable<ComentarioDto>> ExecuteAsync(GuardarComentarioRequest req, string usuarioId)
    {
        var ahora = DateTime.UtcNow;
        var comentarios = req.OrdenCompraIds.Select(ocId => new Comentario
        {
            OrdenCompraId = ocId,
            Texto = req.Texto,
            FechaCompromiso = req.FechaCompromiso,
            GuiaTransporte = req.GuiaTransporte,
            UsuarioId = usuarioId,
            FechaRegistro = ahora,
        });

        var guardados = await repo.AddRangeAsync(comentarios);
        return guardados.Select(g => new ComentarioDto(
            g.Id, g.Texto, g.FechaCompromiso, g.GuiaTransporte, g.FechaRegistro));
    }
}
