import type { OrdenPendiente } from "../types";

interface Props {
  orden: OrdenPendiente | null;
}

function Campo({ label, valor }: { label: string; valor: string | number | null | undefined }) {
  return (
    <div className="detalle-campo">
      <span className="detalle-label">{label}:</span>
      <span className="detalle-valor">{valor ?? "—"}</span>
    </div>
  );
}

export default function OrderDetail({ orden }: Props) {
  if (!orden) {
    return (
      <div className="panel-vacio">
        Selecciona un pedido de la lista para ver su detalle.
      </div>
    );
  }

  return (
    <div className="order-detail">
      <Campo label="Fuente / Finca" valor={orden.fuenteFinca} />
      <Campo label="Documento OC" valor={orden.numeroOc} />
      <Campo label="Código artículo" valor={orden.codigoArt} />
      <Campo label="Descripción" valor={orden.descripcion} />
      <Campo label="Fecha pedido" valor={orden.fechaPedido} />
      <Campo label="Fecha entrega" valor={orden.fechaEntrega} />
      <Campo label="Cantidad pedida" valor={orden.cantidadPedida} />
      <Campo label="Cantidad pendiente" valor={orden.cantidadPend} />
      <Campo label="Obs. compras" valor={orden.obsCompras} />
    </div>
  );
}
