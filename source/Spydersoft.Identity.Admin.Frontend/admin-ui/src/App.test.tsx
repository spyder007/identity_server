import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import App from "./App";

const auth = vi.hoisted(() => ({
  value: { isAuthenticated: true, user: { name: "Ada" }, logout: vi.fn() } as Record<string, unknown>,
}));

vi.mock("./context", () => ({ useAuth: () => auth.value }));
vi.mock("./components/SignInPage", () => ({ default: () => <div>sign in</div> }));
vi.mock("./components/GlobalToast", () => ({ default: () => null }));
vi.mock("./pages/Clients", () => ({ default: () => <div>clients page</div> }));
vi.mock("./pages/ApiResources", () => ({ default: () => null }));
vi.mock("./pages/IdentityResources", () => ({ default: () => null }));
vi.mock("./pages/Scopes", () => ({ default: () => null }));
vi.mock("./pages/Users", () => ({ default: () => null }));
vi.mock("./pages/Roles", () => ({ default: () => null }));

const renderApp = (path = "/") =>
  render(
    <MemoryRouter initialEntries={[path]}>
      <App />
    </MemoryRouter>,
  );

describe("App", () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.setAttribute("data-theme", "light");
  });

  it("shows the sign-in page when unauthenticated", () => {
    auth.value = { isAuthenticated: false, logout: vi.fn() };
    renderApp();
    expect(screen.getByText("sign in")).toBeInTheDocument();
  });

  it("renders navigation and routes when authenticated", () => {
    auth.value = { isAuthenticated: true, user: { name: "Ada" }, logout: vi.fn() };
    renderApp("/clients");
    expect(screen.getByText("clients page")).toBeInTheDocument();
    expect(screen.getByText("Ada")).toBeInTheDocument();
  });

  it("toggles the theme from the sidebar", async () => {
    auth.value = { isAuthenticated: true, user: { name: "Ada" }, logout: vi.fn() };
    renderApp();
    await userEvent.click(screen.getByRole("button", { name: "Toggle theme" }));
    expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
  });
});
