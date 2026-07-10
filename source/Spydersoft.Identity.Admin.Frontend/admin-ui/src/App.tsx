import { NavLink, Route, Routes } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faIdBadge,
  faKey,
  faRightFromBracket,
  faServer,
  faShield,
  faShieldHalved,
  faUsers,
  faUserShield,
} from "@fortawesome/free-solid-svg-icons";

import Clients from "./pages/Clients";
import ApiResources from "./pages/ApiResources";
import IdentityResources from "./pages/IdentityResources";
import Scopes from "./pages/Scopes";
import Users from "./pages/Users";
import Roles from "./pages/Roles";
import GlobalToast from "./components/GlobalToast";
import SignInPage from "./components/SignInPage";
import { useAuth } from "./context";

interface NavItem {
  to: string;
  label: string;
  icon: typeof faIdBadge;
}

const navGroups: { heading: string; items: NavItem[] }[] = [
  {
    heading: "Configuration",
    items: [
      { to: "/clients", label: "Clients", icon: faIdBadge },
      { to: "/api-resources", label: "API Resources", icon: faServer },
      { to: "/identity-resources", label: "Identity Resources", icon: faShield },
      { to: "/scopes", label: "Scopes", icon: faKey },
    ],
  },
  {
    heading: "People",
    items: [
      { to: "/users", label: "Users", icon: faUsers },
      { to: "/roles", label: "Roles", icon: faUserShield },
    ],
  },
];

export default function App() {
  const { isAuthenticated, user, logout } = useAuth();

  if (!isAuthenticated) {
    return <SignInPage />;
  }

  const userName = user?.name || "Signed in";
  const userInitial = userName.charAt(0).toUpperCase() || "?";

  return (
    <div className="flex h-screen overflow-hidden bg-surface-muted">
      <GlobalToast />

      <aside className="flex w-60 flex-col border-r border-border bg-surface">
        <div className="flex items-center gap-2.5 px-5 py-4">
          <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-brand text-brand-fg shadow-sm">
            <FontAwesomeIcon icon={faShieldHalved} />
          </span>
          <div className="leading-tight">
            <div className="text-[0.95rem] font-semibold text-content">Identity</div>
            <div className="text-[0.7rem] font-medium uppercase tracking-wider text-content-subtle">
              Admin
            </div>
          </div>
        </div>

        <nav className="flex-1 overflow-y-auto px-3 pt-2 pb-4">
          {navGroups.map((group) => (
            <div key={group.heading} className="mb-5">
              <div className="px-3 pb-1.5 text-[0.65rem] font-semibold uppercase tracking-[0.08em] text-content-subtle">
                {group.heading}
              </div>
              <div className="flex flex-col gap-0.5">
                {group.items.map(({ to, label, icon }) => (
                  <NavLink
                    key={to}
                    to={to}
                    className={({ isActive }) =>
                      [
                        "flex items-center gap-3 rounded-md px-3 py-2 text-sm transition-colors",
                        isActive
                          ? "bg-brand-soft font-medium text-brand"
                          : "text-content-muted hover:bg-surface-muted hover:text-content",
                      ].join(" ")
                    }
                  >
                    <FontAwesomeIcon icon={icon} className="w-4 text-[0.875rem]" />
                    {label}
                  </NavLink>
                ))}
              </div>
            </div>
          ))}
        </nav>

        <div className="border-t border-border px-3 py-3">
          <div className="flex items-center gap-2.5 rounded-md px-2 py-2">
            <span className="flex h-8 w-8 items-center justify-center rounded-full bg-brand-soft text-sm font-semibold text-brand">
              {userInitial}
            </span>
            <div className="min-w-0 flex-1">
              <div className="truncate text-sm font-medium text-content">{userName}</div>
              <div className="text-[0.7rem] text-content-subtle">Administrator</div>
            </div>
            <button
              type="button"
              onClick={logout}
              aria-label="Sign out"
              className="rounded-md p-2 text-content-subtle transition-colors hover:bg-surface-muted hover:text-danger"
            >
              <FontAwesomeIcon icon={faRightFromBracket} />
            </button>
          </div>
        </div>
      </aside>

      <main className="flex-1 overflow-y-auto">
        <div className="mx-auto w-full max-w-6xl px-6 py-8 md:px-8">
          <Routes>
            <Route path="/clients/*" element={<Clients />} />
            <Route path="/api-resources/*" element={<ApiResources />} />
            <Route path="/identity-resources/*" element={<IdentityResources />} />
            <Route path="/scopes/*" element={<Scopes />} />
            <Route path="/users/*" element={<Users />} />
            <Route path="/roles/*" element={<Roles />} />
            <Route
              path="*"
              element={
                <div className="text-content-muted">
                  Select a section from the sidebar.
                </div>
              }
            />
          </Routes>
        </div>
      </main>
    </div>
  );
}
