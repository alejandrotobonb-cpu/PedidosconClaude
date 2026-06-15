using Microsoft.AspNetCore.Mvc;

namespace Portal.API.Controllers;

/// <summary>
/// Simula el endpoint de SAG para desarrollo local.
/// Solo disponible en entorno Development.
/// Contrato: GET /api/sag-mock/oc/pendientes?nit={nit}
/// </summary>
[ApiController]
[Route("api/oc")]
public class SagMockController : ControllerBase
{
    [HttpGet("pendientes")]
    public IActionResult GetOrdenesPendientes([FromQuery] string nit)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var ordenes = nit switch
        {
            "900123456" => OrdenesProveedor1(hoy),
            "800456789" => OrdenesProveedor2(hoy),
            _ => []
        };

        return Ok(new
        {
            proveedor_nit = nit,
            ordenes
        });
    }

    private static object[] OrdenesProveedor1(DateOnly hoy) =>
    [
        new {
            numero_oc = "SAG-001",
            fuente_finca = "Finca El Rosal",
            codigo_articulo = "FERT-001",
            descripcion = "Fertilizante NPK 50kg [desde SAG]",
            fecha_pedido = hoy.AddDays(-60).ToString("yyyy-MM-dd"),
            fecha_entrega = hoy.AddDays(-45).ToString("yyyy-MM-dd"),
            cantidad_pedida = 100,
            cantidad_pendiente = 60,
            observaciones = "Pedido urgente temporada alta",
            urgente = false
        },
        new {
            numero_oc = "SAG-002",
            fuente_finca = "Finca La Esperanza",
            codigo_articulo = "PEST-007",
            descripcion = "Herbicida glifosato 20L [desde SAG]",
            fecha_pedido = hoy.AddDays(-20).ToString("yyyy-MM-dd"),
            fecha_entrega = hoy.AddDays(3).ToString("yyyy-MM-dd"),
            cantidad_pedida = 50,
            cantidad_pendiente = 50,
            observaciones = (string?)null,
            urgente = false
        },
        new {
            numero_oc = "SAG-003",
            fuente_finca = "Finca El Paraíso",
            codigo_articulo = "SEMI-012",
            descripcion = "Semillas de tomate cherry x1000 [desde SAG]",
            fecha_pedido = hoy.AddDays(-10).ToString("yyyy-MM-dd"),
            fecha_entrega = hoy.AddDays(15).ToString("yyyy-MM-dd"),
            cantidad_pedida = 200,
            cantidad_pendiente = 200,
            observaciones = (string?)null,
            urgente = false
        },
        new {
            numero_oc = "SAG-004",
            fuente_finca = "Finca El Rosal",
            codigo_articulo = "RIEGO-003",
            descripcion = "Sistema de riego por goteo [desde SAG]",
            fecha_pedido = hoy.AddDays(-5).ToString("yyyy-MM-dd"),
            fecha_entrega = hoy.AddDays(7).ToString("yyyy-MM-dd"),
            cantidad_pedida = 1,
            cantidad_pendiente = 1,
            observaciones = "Instalación programada semana próxima",
            urgente = true
        }
    ];

    private static object[] OrdenesProveedor2(DateOnly hoy) =>
    [
        new {
            numero_oc = "SAG-101",
            fuente_finca = "Finca Santa Rosa",
            codigo_articulo = "AGRO-021",
            descripcion = "Agroquímico multipropósito [desde SAG]",
            fecha_pedido = hoy.AddDays(-30).ToString("yyyy-MM-dd"),
            fecha_entrega = hoy.AddDays(5).ToString("yyyy-MM-dd"),
            cantidad_pedida = 300,
            cantidad_pendiente = 150,
            observaciones = (string?)null,
            urgente = false
        }
    ];
}
