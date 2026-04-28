import { useState, useMemo } from "react";
import type { ArticuloGroup } from "../types";

type Filter = "all" | "vencidas" | "proximas" | "urgentes";

interface Props {
  groups: ArticuloGroup[];
  activeId: number | null;
  selected: Set<number>;
  onClickRow: (id: number) => void;
  onToggleSel: (id: number) => void;
  onSelectGroup: (ids: number[]) => void;
  onClearSel: () => void;
}

const FILTER_LABELS: [Filter, string][] = [
  ["all", "Todos"],
  ["vencidas", "Vencidos"],
  ["proximas", "≤ 6 días"],
  ["urgentes", "Urgentes"],
];

function daysClass(d: number) {
  if (d < 0) return "days-red";
  if (d <= 6) return "days-orange";
  return "days-green";
}

function daysLabel(d: number) {
  return d < 0 ? `${Math.abs(d)}` : `+${d}`;
}

export default function OcList({ groups, activeId, selected, onClickRow, onToggleSel, onSelectGroup, onClearSel }: Props) {
  const [collapsed, setCollapsed] = useState<Set<string>>(new Set());
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState<Filter>("all");

  const toggleGroup = (key: string) =>
    setCollapsed(prev => {
      const next = new Set(prev);
      next.has(key) ? next.delete(key) : next.add(key);
      return next;
    });

  // Fix #9: search and filter applied here; parent groups prop is unchanged
  const visibleGroups = useMemo(() => {
    const q = search.toLowerCase().trim();
    return groups
      .map(g => ({
        ...g,
        ordenes: g.ordenes.filter(o => {
          const matchFilter =
            filter === "all" ||
            (filter === "vencidas" && o.diasVencimiento < 0) ||
            (filter === "proximas" && o.diasVencimiento >= 0 && o.diasVencimiento <= 6) ||
            (filter === "urgentes" && o.urgente);
          const matchSearch =
            !q ||
            o.articulo.toLowerCase().includes(q) ||
            o.finca.toLowerCase().includes(q) ||
            o.numeroOC.toLowerCase().includes(q);
          return matchFilter && matchSearch;
        }),
      }))
      .filter(g => g.ordenes.length > 0);
  }, [groups, search, filter]);

  return (
    <div className="panel-list">
      <div className="panel-list-header">
        <div className="panel-list-header-title">Pedidos Pendientes</div>
        <input
          className="search-input"
          placeholder="🔍 Buscar artículo, finca, OC…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <div className="filter-chips">
          {FILTER_LABELS.map(([v, l]) => (
            <button
              key={v}
              className={`chip ${filter === v ? "chip-active" : ""}`}
              onClick={() => setFilter(v)}
            >
              {l}
            </button>
          ))}
        </div>
      </div>

      {selected.size > 0 && (
        <div className="multi-bar visible">
          <div className="multi-bar-left">
            <span>✓ {selected.size} OC seleccionadas</span>
            <button className="btn-clear-sel" onClick={onClearSel}>Limpiar</button>
          </div>
          <span style={{ fontSize: 11 }}>Panel derecho: comentario conjunto</span>
        </div>
      )}

      <div className="oc-scroll">
        {visibleGroups.length === 0 && (
          <div className="empty-state" style={{ minHeight: 200 }}>
            <p>Sin resultados para la búsqueda actual</p>
          </div>
        )}
        {visibleGroups.map(({ codigoArticulo, articulo, ordenes }) => {
          const isCollapsed = collapsed.has(codigoArticulo);
          const minDias = Math.min(...ordenes.map(o => o.diasVencimiento));
          const hasUrgente = ordenes.some(o => o.urgente);
          const allSel = ordenes.every(o => selected.has(o.id));

          return (
            <div key={codigoArticulo} className="art-group">
              <div className="art-group-header" onClick={() => toggleGroup(codigoArticulo)}>
                <div className="art-group-left">
                  <span className={`art-group-icon ${isCollapsed ? "collapsed" : ""}`}>▼</span>
                  <div className="art-group-name-wrap">
                    <div className="art-group-name">{articulo}</div>
                    <div className="art-group-code">{codigoArticulo}</div>
                  </div>
                </div>
                <div className="art-group-right">
                  {hasUrgente && <span className="tag tag-urgente">URGENTE</span>}
                  <span className={`art-group-badge ${minDias < 0 ? "danger" : minDias <= 6 ? "warn" : ""}`}>
                    {daysLabel(minDias)}d
                  </span>
                  <span className="art-group-badge" style={{ background: "#555" }}>{ordenes.length} fincas</span>
                  {/* Fix #1: always pass the actual IDs; Dashboard.handleSelectGroup toggles them */}
                  <button
                    className="btn-sel-group"
                    onClick={e => { e.stopPropagation(); onSelectGroup(ordenes.map(o => o.id)); }}
                  >
                    {allSel ? "Desmarcar" : "Seleccionar grupo"}
                  </button>
                </div>
              </div>

              {!isCollapsed && (
                <div className="art-group-items">
                  {[...ordenes].sort((a, b) => a.diasVencimiento - b.diasVencimiento).map(oc => (
                    <div
                      key={oc.id}
                      className={`oc-item ${activeId === oc.id ? "active" : ""} ${selected.has(oc.id) ? "checked" : ""}`}
                      id={`row-${oc.id}`}
                    >
                      <input
                        type="checkbox"
                        className="oc-cb"
                        checked={selected.has(oc.id)}
                        onChange={e => { e.stopPropagation(); onToggleSel(oc.id); }}
                      />
                      <div className={`oc-days ${daysClass(oc.diasVencimiento)}`}>
                        {daysLabel(oc.diasVencimiento)}<small>días</small>
                      </div>
                      <div className="oc-info" onClick={() => onClickRow(oc.id)}>
                        <div className="oc-finca">{oc.finca}</div>
                        <div className="oc-num">{oc.numeroOC} · {oc.cantidadPendiente.toLocaleString()} {oc.unidadMedida}</div>
                        <div className="oc-tags">
                          {oc.urgente && <span className="tag tag-urgente">⚡ URGENTE</span>}
                          {oc.ultimoComentario && <span className="tag tag-comentado">✓ comentado</span>}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
