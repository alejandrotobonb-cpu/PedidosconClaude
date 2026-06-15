using Microsoft.Extensions.Logging;
using Portal.Domain.Entities;
using Portal.Domain.Interfaces;
using System.Net.Http.Json;

namespace Portal.Infrastructure.SagAdapter;

public class SagClient(
    IHttpClientFactory httpClientFactory,
    ILogger<SagClient> logger) : ISagClient
{
    private const int MaxReintentos = 3;

    public async Task<IReadOnlyList<OrdenCompra>> ObtenerOrdenesPendientesAsync(
        string nit, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("SAG");
        Exception? ultimoError = null;

        for (int intento = 1; intento <= MaxReintentos; intento++)
        {
            try
            {
                var response = await client.GetFromJsonAsync<SagOcResponse>(
                    $"/api/oc/pendientes?nit={nit}", ct);

                if (response?.Ordenes is null)
                    return [];

                logger.LogInformation(
                    "SAG → NIT {Nit}: {Count} órdenes (intento {Intento})",
                    nit, response.Ordenes.Count, intento);

                return response.Ordenes.Select(dto => MapearOrden(dto, nit)).ToList();
            }
            catch (HttpRequestException ex) when (intento < MaxReintentos)
            {
                ultimoError = ex;
                var espera = TimeSpan.FromSeconds(Math.Pow(2, intento)); // 2s, 4s
                logger.LogWarning(
                    "SAG → NIT {Nit}: intento {Intento} fallido, reintentando en {Espera}s",
                    nit, intento, espera.TotalSeconds);
                await Task.Delay(espera, ct);
            }
        }

        logger.LogError(ultimoError,
            "SAG → NIT {Nit}: todos los reintentos agotados", nit);
        return [];
    }

    private static OrdenCompra MapearOrden(SagOrdenDto dto, string nit) => new()
    {
        NumeroOc = dto.NumeroOc,
        ProveedorNit = nit,
        FuenteFinca = dto.FuenteFinca,
        CodigoArt = dto.CodigoArticulo,
        Descripcion = dto.Descripcion,
        FechaPedido = dto.FechaPedido,
        FechaEntrega = dto.FechaEntrega,
        CantidadPedida = dto.CantidadPedida,
        CantidadPend = dto.CantidadPendiente,
        ObsCompras = dto.Observaciones,
        Urgente = dto.Urgente,
        SincronizadoEn = DateTime.UtcNow
    };
}
