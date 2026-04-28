import { useMsal } from "@azure/msal-react";

export default function Header() {
  const { instance, accounts } = useMsal();
  const nombre = accounts[0]?.name ?? "";

  return (
    <header className="header">
      <div className="header-left">
        <span className="logo">GHT</span>
        <span className="header-title">Control Pendientes de Entrega – Proveedores</span>
      </div>
      <div className="header-right">
        <span className="user-badge">{nombre}</span>
        <button className="btn-header" onClick={() => instance.logoutPopup()}>
          Cerrar sesión
        </button>
      </div>
    </header>
  );
}
