import { useSyncExternalStore } from "react";
import lightThemeUrl from "primereact/resources/themes/lara-light-indigo/theme.css?url";
import darkThemeUrl from "primereact/resources/themes/lara-dark-indigo/theme.css?url";

export type Theme = "light" | "dark";

const STORAGE_KEY = "spydersoft-theme";
const LINK_ID = "primereact-theme";
const listeners = new Set<() => void>();

const readStored = (): Theme => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === "light" || stored === "dark") return stored;
  } catch {
    // storage unavailable; fall through to system preference
  }
  return window.matchMedia?.("(prefers-color-scheme: dark)").matches ? "dark" : "light";
};

const currentTheme = (): Theme =>
  document.documentElement.dataset.theme === "dark" ? "dark" : "light";

export const setTheme = (theme: Theme): void => {
  document.documentElement.dataset.theme = theme;

  let link = document.getElementById(LINK_ID) as HTMLLinkElement | null;
  if (!link) {
    link = document.createElement("link");
    link.id = LINK_ID;
    link.rel = "stylesheet";
    document.head.appendChild(link);
  }
  link.href = theme === "dark" ? darkThemeUrl : lightThemeUrl;

  try {
    localStorage.setItem(STORAGE_KEY, theme);
  } catch {
    // ignore
  }
  listeners.forEach((l) => l());
};

export const initTheme = (): void => setTheme(readStored());

const subscribe = (listener: () => void) => {
  listeners.add(listener);
  return () => listeners.delete(listener);
};

export function useTheme() {
  const theme = useSyncExternalStore(subscribe, currentTheme);
  return { theme, toggleTheme: () => setTheme(theme === "dark" ? "light" : "dark") };
}
