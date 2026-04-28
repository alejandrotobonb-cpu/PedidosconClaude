using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalProveedores.Application.DTOs;
using PortalProveedores.Application.Interfaces;
using PortalProveedores.Domain.Interfaces;

namespace PortalProveedores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
// Fix #6: inject Application interfaces, not concrete classes
public class OrdenesController(
    IOrdenesPorProveedor ordenesPorProveedor,
    IGuardarComentario guardarComentario,
    IProveedorRepository proveedorRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMisOrdenes()
    {
        var objectId = User.FindFirst("oid")?.Value
                    ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (string.IsNullOrEmpty(objectId))
            return Unauthorized();

        var proveedor = await proveedorRepo.GetByAzureAdObjectIdAsync(objectId);
        if (proveedor is null)
            return Forbid();

        var ordenes = await ordenesPorProveedor.ExecuteAsync(proveedor.Nit);
        return Ok(ordenes);
    }

    [HttpPost("comentarios")]
    public async Task<IActionResult> GuardarComentarios([FromBody] GuardarComentarioRequest request)
    {
        if (!ModelState.IsValid || request.OrdenCompraIds.Count == 0)
            return BadRequest();

        var usuarioId = User.FindFirst("oid")?.Value
                     ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                     ?? string.Empty;

        var resultados = await guardarComentario.ExecuteAsync(request, usuarioId);
        return Ok(resultados);
    }
}
