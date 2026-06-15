import { useState, useEffect } from "react";
import {
  useIsAuthenticated,
  useMsal,
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
} from "@azure/msal-react";
import { loginRequest } from "./authConfig";
import { initApiAuth, setDevMode } from "./services/api";
import InstructivoPage from "./pages/InstructivoPage";
import MainPage from "./pages/MainPage";
import "./App.css";

const DEV_BYPASS = import.meta.env.VITE_DEV_BYPASS === "true";

function App() {
  const isAuthenticated = useIsAuthenticated();
  const { instance, accounts } = useMsal();
  const [mostrarInstructivo, setMostrarInstructivo] = useState(true);

  useEffect(() => {
    if (DEV_BYPASS) {
      setDevMode(true);
      return;
    }
    if (isAuthenticated && accounts[0]) {
      initApiAuth(instance, accounts[0]);
    }
  }, [isAuthenticated, instance, accounts]);

  if (DEV_BYPASS) {
    return mostrarInstructivo ? (
      <InstructivoPage
        usuario="proveedor@dev.local"
        onContinuar={() => setMostrarInstructivo(false)}
      />
    ) : (
      <MainPage
        usuario="proveedor@dev.local"
        onLogout={() => setMostrarInstructivo(true)}
      />
    );
  }

  const handleLogin = () =>
    instance.loginPopup(loginRequest).catch(console.error);
  const handleLogout = () =>
    instance.logoutPopup().catch(console.error);

  return (
    <>
      <UnauthenticatedTemplate>
        <div className="login-screen">
          <div className="login-card">
            <div className="login-logo">GHT</div>
            <h1>Control Pendientes de Entrega</h1>
            <p>Portal de Proveedores — GHT Growers Hub Trading</p>
            <button className="btn-primary btn-login" onClick={handleLogin}>
              Iniciar sesión con cuenta GHT
            </button>
          </div>
        </div>
      </UnauthenticatedTemplate>

      <AuthenticatedTemplate>
        {mostrarInstructivo ? (
          <InstructivoPage
            usuario={accounts[0]?.username ?? ""}
            onContinuar={() => setMostrarInstructivo(false)}
          />
        ) : (
          <MainPage
            usuario={accounts[0]?.username ?? ""}
            onLogout={handleLogout}
          />
        )}
      </AuthenticatedTemplate>
    </>
  );
}

export default App;
