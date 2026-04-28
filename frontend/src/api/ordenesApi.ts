import axios from "axios";
import { GuardarComentarioRequest, OrdenCompraDto, ComentarioDto } from "../types";

const api = axios.create({ baseURL: "http://localhost:5000/api" });

export function setAuthToken(token: string) {
  api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

export async function getMisOrdenes(): Promise<OrdenCompraDto[]> {
  const { data } = await api.get<OrdenCompraDto[]>("/ordenes");
  return data;
}

export async function guardarComentarios(
  req: GuardarComentarioRequest
): Promise<ComentarioDto[]> {
  const { data } = await api.post<ComentarioDto[]>("/ordenes/comentarios", req);
  return data;
}
