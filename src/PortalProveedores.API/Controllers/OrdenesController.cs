using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalProveedores.Application.DTOs;
using PortalProveedores.Application.Interfaces;
using PortalProveedores.Domain.Interfaces;

namespace PortalProveedores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesController(
    IOrdenesPorProveedor ordenesPorProveedor,
    IGuardarComentario guardarComentario,
    IProveedorRepository proveedorRepo,
    IConfiguration config,
    IWebHostEnvironment env) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMisOrdenes()
    {
        var nit = await ResolverNitAsync();
        if (nit is null) return Forbid();

        var ordenes = await ordenesPorProveedor.ExecuteAsync(nit);
        return Ok(ordenes);
    }

    [HttpPost("comentarios")]
    public async Task<IActionResult> GuardarComentarios([FromBody] GuardarComentarioRequest request)
    {
        if (!ModelState.IsValid || request.OrdenCompraIds.Count == 0)
            return BadRequest();

        var nit = await ResolverNitAsync();
        if (nit is null) return Forbid();

        var usuarioId = ObtenerObjectId() ?? nit;
        var resultados = await guardarComentario.ExecuteAsync(request, usuarioId);
        return Ok(resultados);
    }

    // En Development, DevBypassProveedorNit evita la búsqueda por AzureAdObjectId
    private async Task<string?> ResolverNitAsync()
    {
        if (env.IsDevelopment())
        {
            var devNit = config["DevBypassProveedorNit"];
            if (!string.IsNullOrEmpty(devNit))
                return devNit;
        }

        var objectId = ObtenerObjectId();
        if (string.IsNullOrEmpty(objectId)) return null;

        var proveedor = await proveedorRepo.GetByAzureAdObjectIdAsync(objectId);
        return proveedor?.Nit;
    }

    private string? ObtenerObjectId() =>
        User.FindFirst("oid")?.Value
        ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
}
