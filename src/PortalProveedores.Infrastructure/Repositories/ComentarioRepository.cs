using Microsoft.EntityFrameworkCore;
using PortalProveedores.Domain.Entities;
using PortalProveedores.Domain.Interfaces;
using PortalProveedores.Infrastructure.Persistence;

namespace PortalProveedores.Infrastructure.Repositories;

public class ComentarioRepository(AppDbContext db) : IComentarioRepository
{
    public async Task<Comentario> AddAsync(Comentario comentario)
    {
        db.Comentarios.Add(comentario);
        await db.SaveChangesAsync();
        return comentario;
    }

    // Fix #3: single round-trip for bulk inserts instead of N SaveChangesAsync calls
    public async Task<IEnumerable<Comentario>> AddRangeAsync(IEnumerable<Comentario> comentarios)
    {
        var lista = comentarios.ToList();
        db.Comentarios.AddRange(lista);
        await db.SaveChangesAsync();
        return lista;
    }

    public async Task<IEnumerable<Comentario>> GetByOrdenCompraIdAsync(int ordenCompraId) =>
        await db.Comentarios
            .Where(c => c.OrdenCompraId == ordenCompraId)
            .OrderByDescending(c => c.FechaRegistro)
            .ToListAsync();

    public async Task<IEnumerable<Comentario>> GetByProveedorNitAsync(string nit) =>
        await db.Comentarios
            .Include(c => c.OrdenCompra)
            .Where(c => c.OrdenCompra.ProveedorNit == nit)
            .ToListAsync();
}
