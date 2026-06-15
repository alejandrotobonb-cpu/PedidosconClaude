import { useState, useEffect, useCallback } from "react";
import type { OrdenPendiente } from "../types";
import { api } from "../services/api";
import AlertIndicator from "../components/AlertIndicator";
import OrderList from "../components/OrderList";
import OrderDetail from "../components/OrderDetail";
import CommentForm from "../components/CommentForm";
import ExportButton from "../components/ExportButton";

interface Props {
  usuario: string;
  onLogout: () => void;
}

export default function MainPage({ usuario, onLogout }: Props) {
  const [ordenes, setOrdenes] = useState<OrdenPendiente[]>([]);
  const [seleccionada, setSeleccionada] = useState<OrdenPendiente | null>(null);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const cargarOrdenes = useCallback(async () => {
    setCargando(true);
    setError(null);
    try {
      const data = await api.getOrdenesPendientes();
      setOrdenes(data);
      if (seleccionada) {
        const actualizada = data.find((o) => o.id === seleccionada.id);
        setSeleccionada(actualizada ?? null);
      }
    } catch {
      setError("No se pudieron cargar los pedidos. Intenta recargar la página.");
    } finally {
      setCargando(false);
    }
  }, [seleccionada]);

  useEffect(() => {
    cargarOrdenes();
  }, []);

  const handleSeleccionar = (orden: OrdenPendiente) => {
    setSeleccionada(orden);
  };

  return (
    <div className="main-layout">
      <header className="app-header">
        <span className="header-logo">GHT</span>
        <span className="header-title">
          Control Pendientes de Entrega GHT – Proveedores
        </span>
        <div className="header-actions">
          <span className="header-usuario">{usuario}</span>
          <ExportButton />
          <button className="btn-logout" onClick={onLogout}>
            Cerrar sesión
          </button>
        </div>
      </header>

      {error && (
        <div className="app-error" role="alert">
          {error}
        </div>
      )}

      <div className="paneles-grid">
        {/* Panel 2 — Lista de OC */}
        <aside className="panel panel-lista">
          <h2 className="panel-titulo">Pedidos pendientes</h2>
          <OrderList
            ordenes={ordenes}
            seleccionada={seleccionada?.id ?? null}
            onSeleccionar={handleSeleccionar}
            cargando={cargando}
          />
          {/* Panel 1 — Indicador de días */}
          <div className="panel-indicador">
            <AlertIndicator orden={seleccionada} />
          </div>
        </aside>

        {/* Paneles 3 y 4 */}
        <section className="panel panel-detalle-comentario">
          <div className="panel panel-detalle">
            <h2 className="panel-titulo">Detalle del pedido</h2>
            <OrderDetail orden={seleccionada} />
          </div>

          <div className="panel panel-comentario">
            <h2 className="panel-titulo">Comentario del proveedor</h2>
            <CommentForm
              orden={seleccionada}
              onGuardado={cargarOrdenes}
            />
          </div>
        </section>
      </div>
    </div>
  );
}
