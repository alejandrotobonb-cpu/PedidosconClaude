using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Application.OrdenesPendientes.Queries;
using System.Security.Claims;

namespace Portal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesController(IMediator mediator) : ControllerBase
{
    [HttpGet("pendientes")]
    public async Task<IActionResult> GetPendientes(CancellationToken ct)
    {
        var nit = User.FindFirstValue("extension_Nit")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var resultado = await mediator.Send(new GetOrdenesPendientesQuery(nit), ct);
        return Ok(resultado);
    }
}
