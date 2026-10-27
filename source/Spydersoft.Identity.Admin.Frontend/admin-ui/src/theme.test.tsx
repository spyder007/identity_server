import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { act, render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { initTheme, setTheme, useTheme } from "./theme";

const Probe = () => {
  const { theme, toggleTheme } = useTheme();
  return (
    <button type="button" onClick={toggleTheme}>
      {theme}
    </button>
  );
};

describe("theme", () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute("data-theme");
    document.getElementById("primereact-theme")?.remove();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it("setTheme sets the attribute, persists, and creates a single theme link", () => {
    setTheme("dark");
    setTheme("light");
    setTheme("dark");

    expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
    expect(localStorage.getItem("spydersoft-theme")).toBe("dark");
    expect(document.querySelectorAll("#primereact-theme")).toHaveLength(1);
  });

  it("initTheme uses the stored theme", () => {
    localStorage.setItem("spydersoft-theme", "dark");
    initTheme();
    expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
  });

  it("initTheme falls back to light when nothing is stored and system is not dark", () => {
    initTheme();
    expect(document.documentElement.getAttribute("data-theme")).toBe("light");
  });

  it("initTheme falls back to the system preference", () => {
    const original = window.matchMedia;
    window.matchMedia = ((q: string) => ({ matches: q.includes("dark") })) as typeof window.matchMedia;
    try {
      initTheme();
      expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
    } finally {
      window.matchMedia = original;
    }
  });

  it("useTheme reflects and toggles the current theme", async () => {
    act(() => setTheme("light"));
    render(<Probe />);
    expect(screen.getByRole("button")).toHaveTextContent("light");

    await userEvent.click(screen.getByRole("button"));
    expect(screen.getByRole("button")).toHaveTextContent("dark");
    expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
  });
});
