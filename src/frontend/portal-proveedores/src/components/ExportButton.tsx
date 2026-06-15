import { useState } from "react";
import { api } from "../services/api";

export default function ExportButton() {
  const [exportando, setExportando] = useState(false);

  const handleExportar = async () => {
    setExportando(true);
    try {
      const blob = await api.exportarExcel();
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `comentarios_${new Date().toISOString().split("T")[0]}.xlsx`;
      a.click();
      URL.revokeObjectURL(url);
    } catch {
      alert("Error al exportar. Intenta de nuevo.");
    } finally {
      setExportando(false);
    }
  };

  return (
    <button
      className="btn-export"
      onClick={handleExportar}
      disabled={exportando}
      aria-label="Exportar comentarios a Excel"
    >
      {exportando ? "Exportando..." : "📥 Exportar Excel"}
    </button>
  );
}
