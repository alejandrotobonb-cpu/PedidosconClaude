using PortalProveedores.Application.DTOs;

namespace PortalProveedores.Application.Interfaces;

public interface IGuardarComentario
{
    Task<IEnumerable<ComentarioDto>> ExecuteAsync(GuardarComentarioRequest req, string usuarioId);
}
