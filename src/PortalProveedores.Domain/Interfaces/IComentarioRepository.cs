using PortalProveedores.Domain.Entities;

namespace PortalProveedores.Domain.Interfaces;

public interface IComentarioRepository
{
    Task<Comentario> AddAsync(Comentario comentario);
    Task<IEnumerable<Comentario>> AddRangeAsync(IEnumerable<Comentario> comentarios);
    Task<IEnumerable<Comentario>> GetByOrdenCompraIdAsync(int ordenCompraId);
    Task<IEnumerable<Comentario>> GetByProveedorNitAsync(string nit);
}
