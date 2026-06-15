import type { OrdenPendiente } from "../types";
import { getEstadoOrden } from "../types";

interface Props {
  orden: OrdenPendiente | null;
}

export default function AlertIndicator({ orden }: Props) {
  if (!orden) {
    return (
      <div className="alert-indicator alert-vacio">
        <span className="alert-dias">—</span>
        <span className="alert-label">Selecciona un pedido</span>
      </div>
    );
  }

  const estado = getEstadoOrden(orden);
  const dias = orden.diasVencimiento;

  const claseEstado = {
    vencida: "alert-vencida",
    urgente: "alert-urgente",
    proxima: "alert-proxima",
    normal: "alert-normal",
  }[estado];

  const signo = dias > 0 ? `+${dias}` : `${dias}`;

  return (
    <div className={`alert-indicator ${claseEstado}`}>
      <span className="alert-dias">{signo}</span>
      <span className="alert-sublabel">días</span>
      {estado === "urgente" && (
        <span className="badge-urgente">URGENTE</span>
      )}
      {estado === "vencida" && (
        <span className="alert-etiqueta">VENCIDO</span>
      )}
      {estado === "proxima" && (
        <span className="alert-etiqueta">PRÓXIMO</span>
      )}
    </div>
  );
}
