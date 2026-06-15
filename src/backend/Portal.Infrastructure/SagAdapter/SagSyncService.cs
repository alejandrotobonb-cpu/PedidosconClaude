using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Domain.Entities;
using Portal.Domain.Interfaces;
using Portal.Infrastructure.Persistence;

namespace Portal.Infrastructure.SagAdapter;

public class SagSyncService(
    IServiceScopeFactory scopeFactory,
    ILogger<SagSyncService> logger) : BackgroundService, ISagSyncService
{
    private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(15);
    private SagSyncResult? _ultimoResultado;

    public SagSyncResult? UltimoResultado => _ultimoResultado;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SAG Sync Service iniciado — intervalo {Min} min", _intervalo.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            _ultimoResultado = await SincronizarAhoraAsync(stoppingToken);
            await Task.Delay(_intervalo, stoppingToken);
        }
    }

    public async Task<SagSyncResult> SincronizarAhoraAsync(CancellationToken ct = default)
    {
        var inicio = DateTime.UtcNow;
        var detalle = new List<string>();
        int procesados = 0, insertadas = 0, actualizadas = 0, errores = 0;

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
        var sagClient = scope.ServiceProvider.GetRequiredService<ISagClient>();

        var nits = await db.Proveedores
            .Where(p => p.Activo)
            .Select(p => p.Nit)
            .ToListAsync(ct);

        logger.LogInformation("SAG Sync → {Count} proveedores activos", nits.Count);

        foreach (var nit in nits)
        {
            try
            {
                var ordenes = await sagClient.ObtenerOrdenesPendientesAsync(nit, ct);
                var (ins, act) = await UpsertOrdenesAsync(db, ordenes, ct);
                insertadas += ins;
                actualizadas += act;
                procesados++;
                detalle.Add($"NIT {nit}: {ins} nuevas, {act} actualizadas");
            }
            catch (Exception ex)
            {
                errores++;
                var msg = $"NIT {nit}: ERROR — {ex.Message}";
                detalle.Add(msg);
                logger.LogError(ex, "SAG Sync → {Msg}", msg);
            }
        }

        var resultado = new SagSyncResult(
            procesados, insertadas, actualizadas, errores,
            inicio, DateTime.UtcNow, detalle);

        logger.LogInformation(
            "SAG Sync completado — {P} proveedores, {I} insertadas, {A} actualizadas, {E} errores — {Ms}ms",
            procesados, insertadas, actualizadas, errores,
            (resultado.Fin - inicio).TotalMilliseconds);

        return resultado;
    }

    private static async Task<(int insertadas, int actualizadas)> UpsertOrdenesAsync(
        PortalDbContext db, IReadOnlyList<OrdenCompra> ordenes, CancellationToken ct)
    {
        int ins = 0, act = 0;

        foreach (var oc in ordenes)
        {
            var existente = await db.OrdenesCompra.FirstOrDefaultAsync(
                o => o.NumeroOc == oc.NumeroOc && o.CodigoArt == oc.CodigoArt, ct);

            if (existente is null)
            {
                db.OrdenesCompra.Add(oc);
                ins++;
            }
            else
            {
                existente.FuenteFinca    = oc.FuenteFinca;
                existente.Descripcion    = oc.Descripcion;
                existente.FechaPedido    = oc.FechaPedido;
                existente.FechaEntrega   = oc.FechaEntrega;
                existente.CantidadPedida = oc.CantidadPedida;
                existente.CantidadPend   = oc.CantidadPend;
                existente.ObsCompras     = oc.ObsCompras;
                existente.Urgente        = oc.Urgente;
                existente.SincronizadoEn = DateTime.UtcNow;
                act++;
            }
        }

        await db.SaveChangesAsync(ct);
        return (ins, act);
    }
}
