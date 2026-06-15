namespace Portal.Domain.Entities;

public class Comentario
{
    public int Id { get; set; }
    public int OrdenCompraId { get; set; }
    public string ProveedorNit { get; set; } = default!;
    public string Texto { get; set; } = default!;
    public DateOnly? FechaCompromiso { get; set; }
    public string? NumeroGuia { get; set; }
    public bool Notificado { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public OrdenCompra OrdenCompra { get; set; } = default!;
    public Proveedor Proveedor { get; set; } = default!;
    public ICollection<Adjunto> Adjuntos { get; set; } = [];
}
