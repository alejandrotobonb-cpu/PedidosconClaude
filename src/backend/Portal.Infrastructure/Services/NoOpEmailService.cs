using Microsoft.Extensions.Logging;
using Portal.Domain.Interfaces;

namespace Portal.Infrastructure.Services;

public class NoOpEmailService(ILogger<NoOpEmailService> logger) : IEmailService
{
    public Task EnviarNotificacionCompradorAsync(
        string compradorEmail, string proveedorNombre, string numeroOc,
        string fuenteFinca, string descripcionArticulo, string textoComentario,
        DateOnly? fechaCompromiso, string? numeroGuia,
        IEnumerable<string> nombresAdjuntos,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[DEV] Email omitido → {Comprador} | OC {NumeroOc} | {Proveedor}",
            compradorEmail, numeroOc, proveedorNombre);
        return Task.CompletedTask;
    }
}
