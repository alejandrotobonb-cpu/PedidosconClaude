using Azure.Communication.Email;
using Portal.Domain.Interfaces;

namespace Portal.Infrastructure.Services;

public class EmailService(EmailClient emailClient, string senderAddress) : IEmailService
{
    public async Task EnviarNotificacionCompradorAsync(
        string compradorEmail,
        string proveedorNombre,
        string numeroOc,
        string fuenteFinca,
        string descripcionArticulo,
        string textoComentario,
        DateOnly? fechaCompromiso,
        string? numeroGuia,
        IEnumerable<string> nombresAdjuntos,
        CancellationToken cancellationToken = default)
    {
        var adjuntosTexto = nombresAdjuntos.Any()
            ? string.Join(", ", nombresAdjuntos)
            : "Sin adjuntos";

        var cuerpo = $"""
            El proveedor {proveedorNombre} ha registrado un comentario en el portal:

              Fuente/Finca    : {fuenteFinca}
              N° OC           : {numeroOc}
              Artículo        : {descripcionArticulo}
              Comentario      : {textoComentario}
              Fecha compromiso: {(fechaCompromiso.HasValue ? fechaCompromiso.Value.ToString("yyyy-MM-dd") : "No indicada")}
              N° Guía         : {numeroGuia ?? "No indicado"}
              Adjuntos        : {adjuntosTexto}

            Ingresa al portal para ver el detalle completo.
            """;

        var message = new EmailMessage(
            senderAddress: senderAddress,
            recipientAddress: compradorEmail,
            content: new EmailContent($"[Portal GHT] Actualización de pedido – {proveedorNombre} – OC {numeroOc}")
            {
                PlainText = cuerpo
            });

        await emailClient.SendAsync(Azure.WaitUntil.Started, message, cancellationToken);
    }
}
