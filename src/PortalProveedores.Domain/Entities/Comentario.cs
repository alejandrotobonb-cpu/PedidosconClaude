namespace PortalProveedores.Domain.Entities;

public class Comentario
{
    public int Id { get; set; }
    public int OrdenCompraId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime? FechaCompromiso { get; set; }
    public string? GuiaTransporte { get; set; }
    public string ProveedorNit { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public bool Notificado { get; set; } = false;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public OrdenCompra OrdenCompra { get; set; } = null!;
    public ICollection<Adjunto> Adjuntos { get; set; } = new List<Adjunto>();
}
