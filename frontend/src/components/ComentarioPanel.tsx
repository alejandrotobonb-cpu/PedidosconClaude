import { useState } from "react";
import { OrdenCompraDto, GuardarComentarioRequest } from "../types";
import { guardarComentarios } from "../api/ordenesApi";

interface Props {
  selectedOcs: OrdenCompraDto[];
  activeOc: OrdenCompraDto | null;
  onSaved: () => void;
}

export default function ComentarioPanel({ selectedOcs, activeOc, onSaved }: Props) {
  const [texto, setTexto] = useState("");
  const [fecha, setFecha] = useState("");
  const [guia, setGuia] = useState("");
  const [saving, setSaving] = useState(false);
  const [toast, setToast] = useState("");

  const isBulk = selectedOcs.length > 1;
  const targetOcs = isBulk ? selectedOcs : activeOc ? [activeOc] : [];

  const showToast = (msg: string) => {
    setToast(msg);
    setTimeout(() => setToast(""), 3000);
  };

  const handleSave = async () => {
    if (!texto.trim() || targetOcs.length === 0) return;
    setSaving(true);
    const req: GuardarComentarioRequest = {
      ordenCompraIds: targetOcs.map(o => o.id),
      texto,
      fechaCompromiso: fecha || null,
      guiaTransporte: guia || null,
    };
    try {
      await guardarComentarios(req);
      showToast(isBulk ? `Guardado en ${targetOcs.length} OC` : "Comentario guardado");
      setTexto(""); setFecha(""); setGuia("");
      onSaved();
    } catch {
      showToast("Error al guardar");
    } finally {
      setSaving(false);
    }
  };

  if (targetOcs.length === 0) {
    return (
      <div className="panel-right">
        <div className="empty-state">
          <div style={{ fontSize: 48 }}>📋</div>
          <p>Selecciona una OC para ver el detalle<br />o usa los checkboxes para comentar varias a la vez</p>
        </div>
      </div>
    );
  }

  return (
    <div className="panel-right">
      {toast && <div className="toast">{toast}</div>}

      {/* Detalle / resumen */}
      <div className="panel-card">
        <div className="panel-card-header">
          {isBulk ? `${targetOcs.length} OC seleccionadas` : "Detalle del pedido"}
        </div>
        {isBulk ? (
          <div className="bulk-banner">
            ✓ Mismo comentario se aplicará a <strong>{targetOcs.length} órdenes de compra</strong>
          </div>
        ) : activeOc && (
          <div className="detail-grid">
            <span>OC</span><span>{activeOc.numeroOC}</span>
            <span>Artículo</span><span>{activeOc.articulo}</span>
            <span>Finca / Destino</span><span>{activeOc.finca}</span>
            <span>Cantidad pendiente</span><span>{activeOc.cantidadPendiente.toLocaleString()} {activeOc.unidadMedida}</span>
            <span>Fecha entrega</span>
            <span style={{ color: activeOc.diasVencimiento < 0 ? "#C62828" : activeOc.diasVencimiento <= 6 ? "#E65100" : "#2E7D32", fontWeight: 700 }}>
              {new Date(activeOc.fechaEntrega).toLocaleDateString("es-CO")}
              {" "}({activeOc.diasVencimiento < 0 ? `${Math.abs(activeOc.diasVencimiento)}d vencida` : `${activeOc.diasVencimiento}d restantes`})
            </span>
          </div>
        )}
      </div>

      {/* Formulario comentario */}
      <div className="panel-card">
        <div className="panel-card-header">Registrar comentario / novedad</div>
        <div style={{ padding: "16px 20px", display: "flex", flexDirection: "column", gap: 12 }}>
          <label className="field-label">Comentario *</label>
          <textarea
            className="field-textarea"
            rows={4}
            placeholder="Describa la novedad, estado de despacho, motivo de demora…"
            value={texto}
            onChange={e => setTexto(e.target.value)}
          />
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <div>
              <label className="field-label">Fecha compromiso</label>
              <input type="date" className="field-input" value={fecha} onChange={e => setFecha(e.target.value)} />
            </div>
            <div>
              <label className="field-label">Guía / tracking</label>
              <input type="text" className="field-input" placeholder="Nº de guía" value={guia} onChange={e => setGuia(e.target.value)} />
            </div>
          </div>
          <button className="btn-save" onClick={handleSave} disabled={saving || !texto.trim()}>
            {saving ? "Guardando…" : isBulk ? `Guardar en ${targetOcs.length} OC` : "Guardar comentario"}
          </button>
        </div>
      </div>
    </div>
  );
}
