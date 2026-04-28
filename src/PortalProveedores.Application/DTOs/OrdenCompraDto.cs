namespace PortalProveedores.Application.DTOs;

public record OrdenCompraDto(
    int Id,
    string NumeroOC,
    string Articulo,
    string CodigoArticulo,
    string Finca,
    int CantidadPendiente,
    string UnidadMedida,
    DateTime FechaEntrega,
    int DiasVencimiento,
    bool Urgente,
    ComentarioDto? UltimoComentario
);

public record ComentarioDto(
    int Id,
    string Texto,
    DateTime? FechaCompromiso,
    string? GuiaTransporte,
    DateTime FechaRegistro
);

public record GuardarComentarioRequest(
    List<int> OrdenCompraIds,
    string Texto,
    DateTime? FechaCompromiso,
    string? GuiaTransporte
);
