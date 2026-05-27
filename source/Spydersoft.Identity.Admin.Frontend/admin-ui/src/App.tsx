import { NavLink, Route, Routes } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faIdBadge,
  faKey,
  faServer,
  faShield,
  faUsers,
} from "@fortawesome/free-solid-svg-icons";

import Clients from "./pages/Clients";
import ApiResources from "./pages/ApiResources";
import IdentityResources from "./pages/IdentityResources";
import Scopes from "./pages/Scopes";
import Users from "./pages/Users";

const navItems = [
  { to: "/clients", label: "Clients", icon: faIdBadge },
  { to: "/api-resources", label: "API Resources", icon: faServer },
  { to: "/identity-resources", label: "Identity Resources", icon: faShield },
  { to: "/scopes", label: "Scopes", icon: faKey },
  { to: "/users", label: "Users", icon: faUsers },
];

export default function App() {
  return (
    <div className="flex h-screen overflow-hidden">
      {/* Sidebar */}
      <aside className="flex w-56 flex-col gap-1 bg-[var(--color-surface)] p-4 shadow-sm">
        <div className="mb-6 px-2 text-lg font-semibold text-[var(--color-content)]">
          Identity Admin
        </div>
        {navItems.map(({ to, label, icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              [
                "flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors",
                isActive
                  ? "bg-[var(--color-brand-tint)] font-medium text-[var(--color-brand)]"
                  : "text-[var(--color-content-muted)] hover:bg-[var(--color-surface-muted)]",
              ].join(" ")
            }
          >
            <FontAwesomeIcon icon={icon} className="w-4" />
            {label}
          </NavLink>
        ))}
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-y-auto p-6">
        <Routes>
          <Route path="/clients/*" element={<Clients />} />
          <Route path="/api-resources/*" element={<ApiResources />} />
          <Route path="/identity-resources/*" element={<IdentityResources />} />
          <Route path="/scopes/*" element={<Scopes />} />
          <Route path="/users/*" element={<Users />} />
          <Route
            path="*"
            element={
              <div className="text-[var(--color-content-muted)]">
                Select a section from the sidebar.
              </div>
            }
          />
        </Routes>
      </main>
    </div>
  );
}
