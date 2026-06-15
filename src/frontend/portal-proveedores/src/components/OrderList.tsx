import type { OrdenPendiente } from "../types";
import { getEstadoOrden } from "../types";

interface Props {
  ordenes: OrdenPendiente[];
  seleccionada: number | null;
  onSeleccionar: (orden: OrdenPendiente) => void;
  cargando: boolean;
}

export default function OrderList({
  ordenes,
  seleccionada,
  onSeleccionar,
  cargando,
}: Props) {
  if (cargando) {
    return <div className="lista-estado">Cargando pedidos...</div>;
  }

  if (ordenes.length === 0) {
    return (
      <div className="lista-estado">No hay pedidos pendientes.</div>
    );
  }

  return (
    <ul className="order-list" role="listbox" aria-label="Pedidos pendientes">
      {ordenes.map((orden) => {
        const estado = getEstadoOrden(orden);
        const seleccionado = orden.id === seleccionada;

        const claseItem = [
          "order-item",
          seleccionado ? "order-item--seleccionado" : "",
          `order-item--${estado}`,
        ]
          .filter(Boolean)
          .join(" ");

        return (
          <li
            key={orden.id}
            className={claseItem}
            role="option"
            aria-selected={seleccionado}
            onClick={() => onSeleccionar(orden)}
            tabIndex={0}
            onKeyDown={(e) => e.key === "Enter" && onSeleccionar(orden)}
          >
            <div className="order-item-header">
              <span className="order-badge">
                {estado === "urgente" && "🔴 URGENTE"}
                {estado === "vencida" && `🔴 ${orden.diasVencimiento}d`}
                {estado === "proxima" && `🟠 ${orden.diasVencimiento}d`}
                {estado === "normal" && `⚪ ${orden.diasVencimiento}d`}
              </span>
            </div>
            <div className="order-item-finca">
              {orden.fuenteFinca ?? "—"}
            </div>
            <div className="order-item-oc">OC {orden.numeroOc}</div>
            <div className="order-item-art">
              {orden.descripcion ?? orden.codigoArt ?? "—"}
            </div>
            {orden.ultimoComentario && (
              <div className="order-item-comentario">
                💬 {orden.ultimoComentario.slice(0, 40)}…
              </div>
            )}
          </li>
        );
      })}
    </ul>
  );
}
