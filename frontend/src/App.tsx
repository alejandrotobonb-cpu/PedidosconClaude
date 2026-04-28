import { useState, useEffect } from "react";
import { MsalProvider, AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import { PublicClientApplication } from "@azure/msal-browser";
import { msalConfig } from "./auth/msalConfig";
import Header from "./components/Header";
import LoginPage from "./components/LoginPage";
import Dashboard from "./pages/Dashboard";
import "./App.css";

const msalInstance = new PublicClientApplication(msalConfig);

export default function App() {
  const [ready, setReady] = useState(false);

  // Fix #8: MSAL v3+ requires initialize() before any auth operations
  useEffect(() => {
    msalInstance.initialize().then(() => setReady(true));
  }, []);

  if (!ready) return null;

  return (
    <MsalProvider instance={msalInstance}>
      <UnauthenticatedTemplate>
        <LoginPage />
      </UnauthenticatedTemplate>
      <AuthenticatedTemplate>
        <div style={{ display: "flex", flexDirection: "column", height: "100vh" }}>
          <Header />
          <main style={{ flex: 1, overflow: "hidden" }}>
            <Dashboard />
          </main>
        </div>
      </AuthenticatedTemplate>
    </MsalProvider>
  );
}
