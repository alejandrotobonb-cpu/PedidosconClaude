namespace Portal.Domain.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public string Nit { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string EmailSac { get; set; } = default!;
    public string CompradorEmail { get; set; } = default!;
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrdenCompra> OrdenesCompra { get; set; } = [];
    public ICollection<Comentario> Comentarios { get; set; } = [];
}
