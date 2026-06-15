using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Application.Comentarios.Commands;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Portal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComentariosController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Guardar(
        [FromBody] GuardarComentarioRequest request, CancellationToken ct)
    {
        var nit = User.FindFirstValue("extension_Nit")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var id = await mediator.Send(new GuardarComentarioCommand(
            request.OrdenCompraId, nit, request.Texto,
            request.FechaCompromiso, request.NumeroGuia), ct);

        return Ok(new { id });
    }
}

public record GuardarComentarioRequest(
    int OrdenCompraId,
    [Required, MinLength(10)] string Texto,
    DateOnly? FechaCompromiso,
    string? NumeroGuia);
