namespace Portal.Domain.Interfaces;

public interface ISagSyncService
{
    Task<SagSyncResult> SincronizarAhoraAsync(CancellationToken ct = default);
}

public record SagSyncResult(
    int ProveedoresProcesados,
    int OrdenesInsertadas,
    int OrdenesActualizadas,
    int Errores,
    DateTime Inicio,
    DateTime Fin,
    IReadOnlyList<string> Detalle);
