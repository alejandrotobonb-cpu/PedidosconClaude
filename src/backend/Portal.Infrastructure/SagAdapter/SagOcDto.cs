using System.Text.Json.Serialization;

namespace Portal.Infrastructure.SagAdapter;

public record SagOcResponse(
    [property: JsonPropertyName("proveedor_nit")] string ProveedorNit,
    [property: JsonPropertyName("ordenes")] List<SagOrdenDto> Ordenes);

public record SagOrdenDto(
    [property: JsonPropertyName("numero_oc")] string NumeroOc,
    [property: JsonPropertyName("fuente_finca")] string? FuenteFinca,
    [property: JsonPropertyName("codigo_articulo")] string? CodigoArticulo,
    [property: JsonPropertyName("descripcion")] string? Descripcion,
    [property: JsonPropertyName("fecha_pedido")] DateOnly? FechaPedido,
    [property: JsonPropertyName("fecha_entrega")] DateOnly? FechaEntrega,
    [property: JsonPropertyName("cantidad_pedida")] decimal? CantidadPedida,
    [property: JsonPropertyName("cantidad_pendiente")] decimal? CantidadPendiente,
    [property: JsonPropertyName("observaciones")] string? Observaciones,
    [property: JsonPropertyName("urgente")] bool Urgente);
