using PortalProveedores.Application.DTOs;
using PortalProveedores.Domain.Entities;
using PortalProveedores.Domain.Interfaces;

namespace PortalProveedores.Application.UseCases;

public class GuardarComentario(IComentarioRepository repo)
{
    public async Task<IEnumerable<ComentarioDto>> ExecuteAsync(GuardarComentarioRequest req, string usuarioId)
    {
        var resultados = new List<ComentarioDto>();
        foreach (var ocId in req.OrdenCompraIds)
        {
            var comentario = new Comentario
            {
                OrdenCompraId = ocId,
                Texto = req.Texto,
                FechaCompromiso = req.FechaCompromiso,
                GuiaTransporte = req.GuiaTransporte,
                UsuarioId = usuarioId,
                FechaRegistro = DateTime.UtcNow
            };
            var guardado = await repo.AddAsync(comentario);
            resultados.Add(new ComentarioDto(
                guardado.Id, guardado.Texto, guardado.FechaCompromiso,
                guardado.GuiaTransporte, guardado.FechaRegistro));
        }
        return resultados;
    }
}
