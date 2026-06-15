interface Props {
  usuario: string;
  onContinuar: () => void;
}

export default function InstructivoPage({ usuario, onContinuar }: Props) {
  return (
    <div className="page-instructivo">
      <header className="app-header">
        <span className="header-logo">GHT</span>
        <span className="header-title">
          Control Pendientes de Entrega GHT – Proveedores
        </span>
        <button className="btn-continuar" onClick={onContinuar} aria-label="Ir al portal">
          Ir al portal →
        </button>
      </header>

      <main className="instructivo-body">
        <h2>¡Bienvenido!</h2>
        <p className="instructivo-usuario">Usuario activo: {usuario}</p>

        <section className="instructivo-seccion">
          <h3>Esta aplicación te permite:</h3>
          <ul>
            <li>Consultar tus pedidos pendientes de entrega</li>
            <li>Registrar comentarios y fechas compromiso</li>
            <li>Adjuntar documentos de soporte</li>
          </ul>
        </section>

        <section className="instructivo-seccion">
          <h3>👉 Cómo usar la app:</h3>
          <ol>
            <li>Selecciona un pedido de la lista</li>
            <li>Revisa el detalle del pedido</li>
            <li>Ingresa tu comentario y guarda (💾)</li>
          </ol>
        </section>

        <section className="instructivo-aviso">
          <h3>⚠️ Importante:</h3>
          <ul>
            <li>Debes hacer clic en 💾 para guardar tu comentario</li>
            <li>Tu equipo de compras verá la información registrada</li>
            <li>Solo ves la información de tu empresa</li>
            <li>Para novedades urgentes contacta a tu comprador asignado</li>
          </ul>
        </section>

        <button className="btn-primary btn-continuar-main" onClick={onContinuar}>
          Ir al portal →
        </button>
      </main>
    </div>
  );
}
