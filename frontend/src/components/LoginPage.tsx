import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../auth/msalConfig";

export default function LoginPage() {
  const { instance } = useMsal();

  const handleLogin = () => instance.loginPopup(loginRequest);

  return (
    <div className="login-bg">
      <div className="login-card">
        <div className="login-logo">GHT</div>
        <p className="login-subtitle">Growers Hub Trading</p>
        <h2>Portal de Proveedores</h2>
        <p>Control de Pendientes de Entrega</p>
        <button className="btn-login" onClick={handleLogin}>
          Iniciar sesión con cuenta GHT
        </button>
        <p className="ad-note">Use sus credenciales corporativas de Microsoft</p>
      </div>
    </div>
  );
}
