namespace Portal.Domain.Interfaces;

public interface IEmailService
{
    Task EnviarNotificacionCompradorAsync(
        string compradorEmail,
        string proveedorNombre,
        string numeroOc,
        string fuenteFinca,
        string descripcionArticulo,
        string textoComentario,
        DateOnly? fechaCompromiso,
        string? numeroGuia,
        IEnumerable<string> nombresAdjuntos,
        CancellationToken cancellationToken = default);
}
