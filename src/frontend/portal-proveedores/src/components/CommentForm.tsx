import { useState, useEffect } from "react";
import type { OrdenPendiente } from "../types";
import { api } from "../services/api";

interface Props {
  orden: OrdenPendiente | null;
  onGuardado: () => void;
}

export default function CommentForm({ orden, onGuardado }: Props) {
  const [texto, setTexto] = useState("");
  const [fechaCompromiso, setFechaCompromiso] = useState("");
  const [numeroGuia, setNumeroGuia] = useState("");
  const [guardando, setGuardando] = useState(false);
  const [mensaje, setMensaje] = useState<{ tipo: "ok" | "error"; texto: string } | null>(null);

  useEffect(() => {
    setTexto("");
    setFechaCompromiso("");
    setNumeroGuia("");
    setMensaje(null);
  }, [orden?.id]);

  const hoy = new Date().toISOString().split("T")[0];
  const puedeGuardar = texto.trim().length >= 10 && !guardando;

  const handleGuardar = async () => {
    if (!orden || !puedeGuardar) return;
    setGuardando(true);
    setMensaje(null);
    try {
      await api.guardarComentario({
        ordenCompraId: orden.id,
        texto: texto.trim(),
        fechaCompromiso: fechaCompromiso || null,
        numeroGuia: numeroGuia.trim() || null,
      });
      setMensaje({ tipo: "ok", texto: "Comentario guardado. Tu comprador fue notificado." });
      onGuardado();
    } catch {
      setMensaje({ tipo: "error", texto: "Error al guardar. Intenta de nuevo." });
    } finally {
      setGuardando(false);
    }
  };

  if (!orden) {
    return (
      <div className="panel-vacio">
        Selecciona un pedido para registrar un comentario.
      </div>
    );
  }

  return (
    <div className="comment-form">
      <div className="form-grupo">
        <label htmlFor="comentario" className="form-label">
          Comentario <span className="requerido">*</span>
        </label>
        <textarea
          id="comentario"
          className="form-textarea"
          value={texto}
          onChange={(e) => setTexto(e.target.value)}
          rows={4}
          minLength={10}
          placeholder="Describe el estado del pedido, novedad o información relevante (mín. 10 caracteres)..."
          aria-required="true"
        />
        {texto.length > 0 && texto.length < 10 && (
          <span className="form-hint">Mínimo 10 caracteres ({texto.length}/10)</span>
        )}
      </div>

      <div className="form-fila">
        <div className="form-grupo">
          <label htmlFor="fechaCompromiso" className="form-label">
            Fecha compromiso
          </label>
          <input
            id="fechaCompromiso"
            type="date"
            className="form-input"
            value={fechaCompromiso}
            min={hoy}
            onChange={(e) => setFechaCompromiso(e.target.value)}
          />
        </div>

        <div className="form-grupo">
          <label htmlFor="numeroGuia" className="form-label">
            N° Guía / Remisión
          </label>
          <input
            id="numeroGuia"
            type="text"
            className="form-input"
            value={numeroGuia}
            onChange={(e) => setNumeroGuia(e.target.value)}
            placeholder="Número de guía o remisión"
          />
        </div>
      </div>

      {mensaje && (
        <div className={`form-mensaje form-mensaje--${mensaje.tipo}`} role="alert">
          {mensaje.texto}
        </div>
      )}

      <button
        className="btn-primary btn-guardar"
        onClick={handleGuardar}
        disabled={!puedeGuardar}
        aria-label="Guardar comentario"
      >
        {guardando ? "Guardando..." : "💾 Guardar"}
      </button>
    </div>
  );
}
