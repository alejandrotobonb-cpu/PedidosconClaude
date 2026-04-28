export interface ComentarioDto {
  id: number;
  texto: string;
  fechaCompromiso: string | null;
  guiaTransporte: string | null;
  fechaRegistro: string;
}

export interface OrdenCompraDto {
  id: number;
  numeroOC: string;
  articulo: string;
  codigoArticulo: string;
  finca: string;
  cantidadPendiente: number;
  unidadMedida: string;
  fechaEntrega: string;
  diasVencimiento: number;
  urgente: boolean;
  ultimoComentario: ComentarioDto | null;
}

export interface GuardarComentarioRequest {
  ordenCompraIds: number[];
  texto: string;
  fechaCompromiso: string | null;
  guiaTransporte: string | null;
}

export type ArticuloGroup = {
  codigoArticulo: string;
  articulo: string;
  ordenes: OrdenCompraDto[];
};
