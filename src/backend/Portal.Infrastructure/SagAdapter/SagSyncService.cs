using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Domain.Entities;
using Portal.Infrastructure.Persistence;
using System.Net.Http.Json;

namespace Portal.Infrastructure.SagAdapter;

public class SagSyncService(
    IServiceScopeFactory scopeFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<SagSyncService> logger) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SincronizarTodasLasOcAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error durante sincronización SAG");
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task SincronizarTodasLasOcAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
        var client = httpClientFactory.CreateClient("SAG");

        var proveedores = await db.Proveedores
            .Where(p => p.Activo)
            .Select(p => p.Nit)
            .ToListAsync(ct);

        foreach (var nit in proveedores)
        {
            await SincronizarProveedorAsync(db, client, nit, ct);
        }
    }

    private async Task SincronizarProveedorAsync(
        PortalDbContext db, HttpClient client, string nit, CancellationToken ct)
    {
        var response = await client.GetFromJsonAsync<SagOcResponse>(
            $"/api/oc/pendientes?nit={nit}", ct);

        if (response is null) return;

        foreach (var dto in response.Ordenes)
        {
            var existente = await db.OrdenesCompra
                .FirstOrDefaultAsync(o => o.NumeroOc == dto.NumeroOc && o.CodigoArt == dto.CodigoArticulo, ct);

            if (existente is null)
            {
                db.OrdenesCompra.Add(MapearOrden(dto, nit));
            }
            else
            {
                ActualizarOrden(existente, dto);
            }
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("SAG sync OK para NIT {Nit}: {Count} órdenes", nit, response.Ordenes.Count);
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

    private static void ActualizarOrden(OrdenCompra oc, SagOrdenDto dto)
    {
        oc.FuenteFinca = dto.FuenteFinca;
        oc.Descripcion = dto.Descripcion;
        oc.FechaPedido = dto.FechaPedido;
        oc.FechaEntrega = dto.FechaEntrega;
        oc.CantidadPedida = dto.CantidadPedida;
        oc.CantidadPend = dto.CantidadPendiente;
        oc.ObsCompras = dto.Observaciones;
        oc.Urgente = dto.Urgente;
        oc.SincronizadoEn = DateTime.UtcNow;
    }
}
