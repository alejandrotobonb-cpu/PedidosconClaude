using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Domain.Interfaces;
using Portal.Infrastructure.SagAdapter;

namespace Portal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SagController(ISagSyncService syncService) : ControllerBase
{
    /// <summary>Dispara una sincronización inmediata con SAG.</summary>
    [HttpPost("sincronizar")]
    public async Task<IActionResult> Sincronizar(CancellationToken ct)
    {
        var resultado = await syncService.SincronizarAhoraAsync(ct);
        return Ok(resultado);
    }

    /// <summary>Devuelve el resultado de la última sincronización.</summary>
    [HttpGet("estado")]
    public IActionResult Estado()
    {
        if (syncService is SagSyncService svc && svc.UltimoResultado is { } r)
            return Ok(r);

        return Ok(new { mensaje = "Sin sincronizaciones ejecutadas aún en esta sesión." });
    }
}
