import axios, { AxiosError, type AxiosInstance } from "axios";
import { client as generatedClient } from "./generated/client.gen";
import { notifyUnauthorized } from "../context/authSession";

export type ApiErrorKind = "auth" | "forbidden" | "notfound" | "validation" | "server" | "network";

export interface ApiError {
  kind: ApiErrorKind;
  status?: number;
  message: string;
  details?: unknown;
}

type Listener = (error: ApiError) => void;
const listeners = new Set<Listener>();

export function onApiError(listener: Listener): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

function emit(error: ApiError): void {
  for (const listener of listeners) listener(error);
}

function toApiError(error: unknown): ApiError {
  if (axios.isAxiosError(error)) {
    const ax = error as AxiosError<{ title?: string; detail?: string }>;
    const status = ax.response?.status;
    const detail = ax.response?.data?.detail ?? ax.response?.data?.title ?? ax.message;
    if (status === 401) return { kind: "auth", status, message: "Sign-in required.", details: ax.response?.data };
    if (status === 403) return { kind: "forbidden", status, message: "Requires write scope.", details: ax.response?.data };
    if (status === 404) return { kind: "notfound", status, message: detail ?? "Not found.", details: ax.response?.data };
    if (status && status >= 400 && status < 500) return { kind: "validation", status, message: detail ?? "Request was rejected.", details: ax.response?.data };
    if (status && status >= 500) return { kind: "server", status, message: detail ?? "Server error.", details: ax.response?.data };
    return { kind: "network", message: ax.message };
  }
  return { kind: "network", message: error instanceof Error ? error.message : String(error) };
}

let initialized = false;
let instance: AxiosInstance | null = null;

export function getAxios(): AxiosInstance {
  if (!instance) throw new Error("API client not initialized. Call configureApi() first.");
  return instance;
}

export function configureApi(): void {
  if (initialized) return;
  initialized = true;

  instance = axios.create({
    baseURL: "",
    withCredentials: true,
    headers: { Accept: "application/json" },
  });

  instance.interceptors.response.use(
    (response) => response,
    (error: unknown) => {
      const apiError = toApiError(error);
      emit(apiError);
      if (apiError.kind === "auth") {
        notifyUnauthorized();
      }
      return Promise.reject(error);
    },
  );

  generatedClient.setConfig({
    axios: instance,
    baseURL: "",
    withCredentials: true,
  });
}

export { toApiError };
