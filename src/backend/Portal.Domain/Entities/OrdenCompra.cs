namespace Portal.Domain.Entities;

public class OrdenCompra
{
    public int Id { get; set; }
    public string NumeroOc { get; set; } = default!;
    public string ProveedorNit { get; set; } = default!;
    public string? FuenteFinca { get; set; }
    public string? CodigoArt { get; set; }
    public string? Descripcion { get; set; }
    public DateOnly? FechaPedido { get; set; }
    public DateOnly? FechaEntrega { get; set; }
    public decimal? CantidadPedida { get; set; }
    public decimal? CantidadPend { get; set; }
    public string? ObsCompras { get; set; }
    public bool Urgente { get; set; } = false;
    public DateTime SincronizadoEn { get; set; } = DateTime.UtcNow;

    public int DiasVencimiento =>
        FechaEntrega.HasValue
            ? (int)(FechaEntrega.Value.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow.Date).TotalDays
            : 0;

    public Proveedor Proveedor { get; set; } = default!;
    public ICollection<Comentario> Comentarios { get; set; } = [];
}
