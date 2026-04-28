namespace PortalProveedores.Domain.Entities;

public class Adjunto
{
    public int Id { get; set; }
    public int ComentarioId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamanioBytes { get; set; }
    public string BlobUri { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Comentario Comentario { get; set; } = null!;
}
