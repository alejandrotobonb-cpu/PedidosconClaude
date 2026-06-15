export interface OrdenPendiente {
  id: number;
  numeroOc: string;
  fuenteFinca: string | null;
  codigoArt: string | null;
  descripcion: string | null;
  fechaPedido: string | null;
  fechaEntrega: string | null;
  cantidadPedida: number | null;
  cantidadPend: number | null;
  obsCompras: string | null;
  urgente: boolean;
  diasVencimiento: number;
  ultimoComentario: string | null;
  fechaCompromiso: string | null;
}

export interface GuardarComentarioRequest {
  ordenCompraId: number;
  texto: string;
  fechaCompromiso: string | null;
  numeroGuia: string | null;
}

export type EstadoOrden = "vencida" | "urgente" | "proxima" | "normal";

export function getEstadoOrden(orden: OrdenPendiente): EstadoOrden {
  if (orden.urgente) return "urgente";
  if (orden.diasVencimiento < 0) return "vencida";
  if (orden.diasVencimiento <= 6) return "proxima";
  return "normal";
}
