import { useState } from "react";
import { ArticuloGroup, OrdenCompraDto } from "../types";

interface Props {
  groups: ArticuloGroup[];
  activeId: number | null;
  selected: Set<number>;
  onClickRow: (id: number) => void;
  onToggleSel: (id: number) => void;
  onSelectGroup: (ids: number[]) => void;
  onClearSel: () => void;
}

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

  const toggleGroup = (key: string) =>
    setCollapsed(prev => {
      const next = new Set(prev);
      next.has(key) ? next.delete(key) : next.add(key);
      return next;
    });

  const allGroupIds = (ordenes: OrdenCompraDto[]) => ordenes.map(o => o.id);

  return (
    <div className="panel-list">
      <div className="panel-list-header">
        <div className="panel-list-header-title">Pedidos Pendientes</div>
        <input className="search-input" placeholder="🔍 Buscar artículo, finca, OC…" readOnly />
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
        {groups.map(({ codigoArticulo, articulo, ordenes }) => {
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
                  <button
                    className="btn-sel-group"
                    onClick={e => { e.stopPropagation(); onSelectGroup(allSel ? [] : allGroupIds(ordenes)); }}
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
