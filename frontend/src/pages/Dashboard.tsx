import { useEffect, useState, useMemo, useCallback } from "react";
import { getMisOrdenes } from "../api/ordenesApi";
import type { OrdenCompraDto, ArticuloGroup } from "../types";
import OcList from "../components/OcList";
import ComentarioPanel from "../components/ComentarioPanel";
import { useApiToken } from "../hooks/useApiToken";

function buildGroups(ordenes: OrdenCompraDto[]): ArticuloGroup[] {
  const map = new Map<string, OrdenCompraDto[]>();
  for (const oc of ordenes) {
    const existing = map.get(oc.codigoArticulo) ?? [];
    map.set(oc.codigoArticulo, [...existing, oc]);
  }
  return Array.from(map.entries())
    .map(([codigoArticulo, ocs]) => ({
      codigoArticulo,
      articulo: ocs[0].articulo,
      ordenes: ocs,
    }))
    .sort((a, b) =>
      Math.min(...a.ordenes.map(o => o.diasVencimiento)) -
      Math.min(...b.ordenes.map(o => o.diasVencimiento))
    );
}

export default function Dashboard() {
  useApiToken();

  const [ordenes, setOrdenes] = useState<OrdenCompraDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeId, setActiveId] = useState<number | null>(null);
  const [selected, setSelected] = useState<Set<number>>(new Set());

  const load = useCallback(async () => {
    setError(null);
    try {
      const data = await getMisOrdenes();
      setOrdenes(data);
    } catch {
      setError("No se pudieron cargar los pedidos. Intente de nuevo.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const groups = useMemo(() => buildGroups(ordenes), [ordenes]);

  const activeOc = ordenes.find(o => o.id === activeId) ?? null;
  const selectedOcs = ordenes.filter(o => selected.has(o.id));

  const handleClickRow = (id: number) => setActiveId(id);

  const handleToggleSel = (id: number) => {
    setSelected(prev => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  };

  // Fix #1: was clearing ALL groups when deselecting; now toggles only the passed group's IDs
  const handleSelectGroup = (ids: number[]) => {
    setSelected(prev => {
      const next = new Set(prev);
      const allInGroup = ids.every(id => next.has(id));
      if (allInGroup) {
        ids.forEach(id => next.delete(id));
      } else {
        ids.forEach(id => next.add(id));
      }
      return next;
    });
  };

  const vencidas = ordenes.filter(o => o.diasVencimiento < 0).length;
  const proximas = ordenes.filter(o => o.diasVencimiento >= 0 && o.diasVencimiento <= 6).length;
  const comentadas = ordenes.filter(o => o.ultimoComentario).length;

  if (loading) return <div className="loading">Cargando pedidos…</div>;

  // Fix #2: show actionable error instead of blank screen when API fails
  if (error) return (
    <div className="loading" style={{ flexDirection: "column", gap: 12, color: "#C62828" }}>
      <span style={{ fontSize: 40 }}>⚠️</span>
      <span>{error}</span>
      <button
        className="btn-save"
        style={{ width: "auto", padding: "8px 20px" }}
        onClick={() => { setLoading(true); load(); }}
      >
        Reintentar
      </button>
    </div>
  );

  return (
    <>
      <div className="main-toolbar">
        <div className="toolbar-info">
          <div className="badge-stat"><div className="dot" style={{ background: "#C62828" }} /><span>{vencidas}</span> vencidas</div>
          <div className="badge-stat"><div className="dot" style={{ background: "#E65100" }} /><span>{proximas}</span> ≤ 6 días</div>
          <div className="badge-stat"><div className="dot" style={{ background: "#2E7D32" }} /><span>{comentadas}/{ordenes.length}</span> comentadas</div>
        </div>
        <button className="btn-excel">⬇ Exportar Excel</button>
      </div>

      <div className="panels-grid">
        <OcList
          groups={groups}
          activeId={activeId}
          selected={selected}
          onClickRow={handleClickRow}
          onToggleSel={handleToggleSel}
          onSelectGroup={handleSelectGroup}
          onClearSel={() => setSelected(new Set())}
        />
        <ComentarioPanel
          selectedOcs={selectedOcs}
          activeOc={activeOc}
          onSaved={load}
        />
      </div>
    </>
  );
}
