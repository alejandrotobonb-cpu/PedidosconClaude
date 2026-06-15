namespace Portal.Domain.Entities;

public class Adjunto
{
    public int Id { get; set; }
    public int ComentarioId { get; set; }
    public string NombreArchivo { get; set; } = default!;
    public string TipoMime { get; set; } = default!;
    public int TamanioBytes { get; set; }
    public string BlobUri { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Comentario Comentario { get; set; } = default!;
}
