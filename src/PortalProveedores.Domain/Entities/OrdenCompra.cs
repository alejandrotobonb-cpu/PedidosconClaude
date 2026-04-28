namespace PortalProveedores.Domain.Entities;

public class OrdenCompra
{
    public int Id { get; set; }
    public string NumeroOC { get; set; } = string.Empty;
    public string ProveedorNit { get; set; } = string.Empty;
    public string Articulo { get; set; } = string.Empty;
    public string CodigoArticulo { get; set; } = string.Empty;
    public string Finca { get; set; } = string.Empty;
    public int CantidadPendiente { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public DateTime FechaEntrega { get; set; }
    public bool Urgente { get; set; }
    public DateTime SyncFecha { get; set; }

    public int DiasVencimiento => (int)(FechaEntrega.Date - DateTime.UtcNow.Date).TotalDays;

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
