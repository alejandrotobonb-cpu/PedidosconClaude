import axios from "axios";
import { IPublicClientApplication, AccountInfo } from "@azure/msal-browser";
import { loginRequest, apiConfig } from "../authConfig";
import type { OrdenPendiente, GuardarComentarioRequest } from "../types";

let msalInstance: IPublicClientApplication | null = null;
let activeAccount: AccountInfo | null = null;

export function initApiAuth(
  instance: IPublicClientApplication,
  account: AccountInfo
) {
  msalInstance = instance;
  activeAccount = account;
}

const client = axios.create({ baseURL: apiConfig.baseUrl });

client.interceptors.request.use(async (config) => {
  if (!msalInstance || !activeAccount) return config;
  const result = await msalInstance.acquireTokenSilent({
    ...loginRequest,
    account: activeAccount,
  });
  config.headers.Authorization = `Bearer ${result.accessToken}`;
  return config;
});

export const api = {
  getOrdenesPendientes: (): Promise<OrdenPendiente[]> =>
    client.get<OrdenPendiente[]>("/api/ordenes/pendientes").then((r) => r.data),

  guardarComentario: (body: GuardarComentarioRequest): Promise<{ id: number }> =>
    client.post("/api/comentarios", body).then((r) => r.data),

  exportarExcel: (): Promise<Blob> =>
    client
      .get("/api/ordenes/exportar", { responseType: "blob" })
      .then((r) => r.data),
};
