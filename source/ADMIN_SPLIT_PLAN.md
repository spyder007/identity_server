# Admin Split Plan — `feature/admin-split`

> **Originally saved:** 2025-08-20
> **Last updated:** 2026-05-28
> **Branch:** `feature/admin-split`
> **Purpose:** Reference doc for resuming work across sessions.

---

## Goal

Split the monolithic `Spydersoft.Identity` project into three separately-hosted applications:

| Project | Description | Hosted At |
|---|---|---|
| `Spydersoft.Identity` | Main auth app — login, profile, grants, consent, device flow | `auth.mattgerega.(net/org/com)` |
| `Spydersoft.Identity.Admin.Api` | REST API for managing clients, resources, scopes, users | `admin.auth.mattgerega.(net/org/com)/api` |
| `Spydersoft.Identity.Admin.Frontend` | React SPA + ASP.NET Core BFF host, calls Admin.Api | `admin.auth.mattgerega.(net/org/com)` |

---

## Architecture Decisions

- **Auth:** Admin.Api secured via Bearer token (OAuth2) against `Spydersoft.Identity`
- **Scopes:** `identity:admin:read` (GET) and `identity:admin:write` (POST/PUT/DELETE), under API resource `identity.admin.api`
- **API versioning:** URL segment — `/api/v1/...`
- **OpenAPI:** `Microsoft.AspNetCore.OpenApi` + Scalar UI (dev only), spec for client gen
- **Frontend:** React SPA hosted in an ASP.NET Core BFF project (`Admin.Frontend`); BFF proxies to Admin.Api via YARP
- **API client gen:** `@hey-api/openapi-ts` + `@hey-api/client-axios` (matches `pitstop-ui` pattern; **not** Kiota — the original plan listed Kiota but we standardized on hey-api)
- **Orchestration:** .NET Aspire AppHost wires the whole stack (Postgres + seeder + identity + admin-api + admin-frontend BFF + Vite dev server) with pinned ports and parameter-driven secrets
- **Phased approach:** Old MVC admin controllers/views in `Spydersoft.Identity` stay **untouched** until Admin.Frontend reaches full parity

---

## Existing Projects

| Project | Role |
|---|---|
| `Spydersoft.Identity` | Monolith — auth + old MVC admin (untouched until Phase 3) |
| `Spydersoft.Identity.Core` | Shared models, ViewModels, exceptions, service interfaces |
| `Spydersoft.Identity.Data` | EF DbContexts (`ApplicationDbContext`, Duende config/persisted grant), migrations |
| `Spydersoft.Identity.Tests` | NUnit test project |
| `Spydersoft.Identity.AppHost` | **NEW** — Aspire orchestration (Postgres, seeder, all three apps, Vite) |
| `Spydersoft.Identity.DataSeeder` | **NEW** — one-shot console: applies EF migrations + seeds clients/resources/identity |
| `Spydersoft.Identity.Admin.Api` | **NEW** — REST API (Phase 1) |
| `Spydersoft.Identity.Admin.Frontend` | **NEW** — BFF + React SPA (Phase 2) |

---

## Phase 1 — `Spydersoft.Identity.Admin.Api` ✅ COMPLETE

### What was built

- **Project:** `Spydersoft.Identity.Admin.Api.csproj` — `net10.0`, CPM, references `Core` + `Data`
- **Packages added to `Directory.Packages.props`:** `Asp.Versioning.Http`, `Asp.Versioning.Mvc.ApiExplorer`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`
- **`Program.cs`:** EF (`ConfigurationDbContext` + `ApplicationDbContext`), ASP.NET Core Identity (UserManager/RoleManager), AutoMapper, JwtBearer auth (audience `identity.admin.api`), read/write scope policies, API versioning, OpenAPI + Scalar, Spydersoft.Platform.Hosting (telemetry/Serilog/health checks)
- **`Data/ApiAutoMapperProfile.cs`:** Entity → DTO mappings (separate from Core ViewModels, non-breaking)
- **`AdminApiPolicies`** static class in `Program.cs` with `Read` and `Write` constants

### Controllers (`Controllers/V1/`)

All routes: `/api/v1/[controller]`

| File | Controllers |
|---|---|
| `BaseAdminApiController.cs` | Abstract base — `[ApiController]`, versioned route, `[Authorize(Read)]`, `IMapper` |
| `ClientsController.cs` | `GET/POST /clients`, `GET/PUT/DELETE /clients/{id}` |
| `ClientSubResourceControllers.cs` | `/clients/{clientId}/claims`, `corsorigins`, `granttypes`, `idprestrictions`, `postlogoutredirecturis`, `properties`, `redirecturis`, `scopes`, `secrets` |
| `ApiResourceControllers.cs` | `/apiresources` CRUD + `/apiresources/{id}/claims`, `properties`, `scopes`, `secrets` |
| `IdentityResourceControllers.cs` | `/identityresources` CRUD + `/identityresources/{id}/claims`, `properties` |
| `ScopeControllers.cs` | `/scopes` CRUD + `/scopes/{id}/claims`, `properties` |
| `UsersController.cs` | `/users` CRUD + `/users/{id}/roles` (GET/POST/DELETE) + `/users/{id}/claims` (GET) |
| `RolesController.cs` | `/roles` CRUD + `/roles/{id}/claims` (GET/POST) + `/roles/{id}/claims/{claimType}` (DELETE) |

### DTOs (`Models/`)

```
Models/
  BaseApiDto.cs
  Clients/
    ClientDtos.cs              (ClientSummaryDto, ClientDto, SaveClientDto)
    ClientSubResourceDtos.cs   (Claim, CorsOrigin, GrantType, IdpRestriction, PostLogoutUri, Property, RedirectUri, Scope, Secret — each with Dto + SaveDto)
  ApiResources/
    ApiResourceDtos.cs         (ApiResourceSummaryDto, ApiResourceDto, SaveApiResourceDto)
    ApiResourceSubResourceDtos.cs
  IdentityResources/
    IdentityResourceDtos.cs
    IdentityResourceSubResourceDtos.cs
  Scopes/
    ScopeDtos.cs
    ScopeSubResourceDtos.cs
  Users/
    UserDtos.cs                (UserSummaryDto, UserDto, SaveUserDto, CreateUserDto, UserRoleDto, AssignUserRoleDto, UserClaimDto)
  Roles/
    RoleDtos.cs                (RoleSummaryDto, RoleDto, SaveRoleDto, RoleClaimDto, SaveRoleClaimDto)
```

### Config

- `appsettings.json` — keys: `ConnectionStrings:IdentityConnection`, `IdentityServer:Authority`, `AutoMapper:License`, `Telemetry`, `HealthChecks`
- `appsettings.Development.json` — debug-level Serilog
- Under Aspire, the AppHost overrides `IdentityServer:Authority`, `ConnectionStrings:IdentityConnection`, `AutoMapper:License`, and the HTTP endpoint (port **7030**, unproxied). The original plan's port 34150 / Scalar launchUrl no longer applies when run via AppHost.

---

## Phase 2 — `Spydersoft.Identity.Admin.Frontend` ✅ SCAFFOLD COMPLETE

### Architecture decisions made

- **BFF:** OidcProxy.Net.OpenIdConnect (YARP-based) — exact mirror of `Spydersoft.PitStop.Frontend` pattern
- **SPA:** React 19 + Vite 8 + TypeScript in `admin-ui/` subfolder; linked as `.esproj`
- **UI components:** PrimeReact + Tailwind v4
- **API client gen:** `@hey-api/openapi-ts` + `@hey-api/client-axios`
- **Routing:** React Router v7 (`react-router-dom`)
- **State:** Redux Toolkit available (added in deps; not yet wired)
- **Token strategy:** OidcProxy.Net handles cookie↔Bearer exchange entirely server-side

### What was built

| File | Purpose |
|---|---|
| `Spydersoft.Identity.Admin.Frontend.csproj` | ASP.NET Core host, SpaRoot=admin-ui, port 7040/7041 |
| `Program.cs` | AddOidcProxy + UseOidcProxy, static files, MapFallbackToFile index.html, Spydersoft.Platform.Hosting (telemetry/Serilog/health checks) |
| `appsettings.json` | OidcProxySettings: OIDC authority/clientId/scopes (`openid profile email identity:admin:read identity:admin:write`) + YARP `/api/**` → adminApi @ localhost:7030 |
| `appsettings.Development.json` | Debug-level Serilog overrides |
| `Properties/launchSettings.json` | http:7040 / https:7041 |
| `admin-ui/admin-ui.esproj` | JavaScript SDK project (yarn dev / yarn build) |
| `admin-ui/package.json` | React 19, Vite 8, PrimeReact, Tailwind v4, @hey-api, React Router 7, Redux Toolkit, vitest, FontAwesome 7 |
| `admin-ui/vite.config.mts` | Proxy `^/api`, `^/.auth`, `^/livez`, `^/readyz` → `https://localhost:7041`; dev port 7050; dotnet dev-certs HTTPS |
| `admin-ui/tsconfig.json` + `tsconfig.node.json` | TypeScript strict config |
| `admin-ui/index.html` | Entry HTML with `/config.js` runtime config script |
| `admin-ui/src/styles.css` | Tailwind v4 + PrimeReact theme + blue brand palette (note: this SPA uses its own palette, **not** the DaisyUI v5 identity theme used by the main app) |
| `admin-ui/src/main.tsx` | React root with BrowserRouter + PrimeReactProvider |
| `admin-ui/src/App.tsx` | Sidebar nav layout + React Router routes to 5 admin sections |
| `admin-ui/openapi-ts.config.ts` | @hey-api config: input `./openapi.json` → `src/api/generated` |
| `admin-ui/src/pages/{Clients,ApiResources,IdentityResources,Scopes,Users}.tsx` | Stubs |
| `admin-ui/public/config.js` | Runtime config: `window.APP_CONFIG = { apiBasePath: "/api" }` |

### Ports (7000-7050 reserved range)

| Service | Port |
|---|---|
| Aspire dashboard (http/https) | http://localhost:7000 / https://localhost:7001 |
| Aspire OTLP / Resource service | 7002 / 7003 |
| Postgres | localhost:7010 |
| Identity main app | http://localhost:7020 |
| Admin.Api | http://localhost:7030 |
| Admin.Frontend BFF (http) | http://localhost:7040 |
| Admin.Frontend BFF (https) | https://localhost:7041 |
| Vite dev server | https://localhost:7050 |

---

## Aspire AppHost + DataSeeder ✅ COMPLETE

The original "Next Immediate Actions" list assumed manual `dotnet user-secrets` setup. That has been replaced by the AppHost-driven approach:

### `Spydersoft.Identity.AppHost`

`Program.cs` wires the full stack with pinned ports and parameter-driven secrets:

- **Postgres** (`postgres`, port 7010) with named data volume `identity-pg-data`
- **`identitydb`** database (DB name `identity`) on the postgres resource
- **`admin-frontend-clientsecret`** Aspire parameter (default `"secret"`, override via `dotnet user-secrets set Parameters:admin-frontend-clientsecret <value>` on the AppHost). Single source of truth for the OIDC client secret of `identity.admin.frontend`.
- **`seeder`** — runs to completion before the other apps start. Receives the DB connection + the cleartext admin-frontend secret.
- **`identity`** — main app, port 7020, waits for seeder completion.
- **`admin-api`** — port 7030, waits for `identity`, receives `IdentityServer:Authority` from `identity`'s HTTP endpoint.
- **`admin-frontend`** — BFF, ports 7040/7041, receives OIDC authority + cleartext client secret + YARP destination address (admin-api endpoint) from the AppHost.
- **`admin-ui`** — Vite dev server via `AddViteApp` with `WithYarn`, https port 7050.

Optional user-secret on AppHost: `AutoMapper:License` (empty string falls through to free-tier behavior).

### `Spydersoft.Identity.DataSeeder`

One-shot console app. On startup:

1. Applies EF migrations: `ApplicationDbContext` + `ConfigurationDbContext` + `PersistedGrantDbContext`
2. Idempotently seeds (skips records that already exist by name/ID):
   - Identity resources (OpenId, Profile, Email, Address, Phone, roles)
   - API scopes (`identity:admin:read`, `identity:admin:write`)
   - API resources (`identity.admin.api` with the two scopes; `ShowInDiscoveryDocument = false`)
   - Clients — including `identity.admin.frontend` (AuthorizationCode + PKCE, redirect `https://localhost:7041/oidc/login/callback`, post-logout `https://localhost:7041/oidc/logout`, scopes `openid profile email identity:admin:read identity:admin:write`, offline access)
   - Default identity user/role
3. The cleartext `admin-frontend-clientsecret` from the AppHost is SHA-256-hashed before being stored on the client record, matching what the BFF will present at the token endpoint.

---

## Phase 3 — Strip old MVC Admin from `Spydersoft.Identity` ✅ COMPLETE (2026-06-01)

`Admin.Frontend` reached full parity (deployment verified), so the old MVC admin was removed from `Spydersoft.Identity`:

- ✅ Removed `Controllers/Admin/*` (Clients + sub-resources, ApiResources, IdentityResources, Scopes) — including `BaseAdminController` and `BaseAdminCollectionController`.
- ✅ Removed `Controllers/UserAdmin/*` (Users, UserRoles, `BaseUserAdminController`).
- ✅ Removed the 22 corresponding View folders and the 4 admin nav-bar partials (`_apiNavBar`, `_clientNavBar`, `_identityResourceNavBar`, `_scopeNavBar`). `_ScopeListItem.cshtml` was **kept** — it is shared with `Consent/Index.cshtml`.
- ✅ Removed the "Administration" section (Identity Server + User Management groups) from `Views/Shared/_SidebarNav.cshtml`. The `isAdmin` admin badge in the footer was left in place. No admin link/home card existed elsewhere.
- ✅ Removed the now-unused `IsISAdminRoute` / `IsUserAdminRoute` helpers from `Extensions/NavHelperExtensions.cs` (`IsSpydersoftRoute` stays — still used by the About section).
- ✅ Removed the stale `<Folder Include="Models\UserAdmin\" />` from the csproj.

**`ConfigurationDbContext` registration was NOT removed (the original plan bullet was incorrect).** There is no standalone `AddDbContext<ConfigurationDbContext>` in `Program.cs`; the configuration store is provided by IdentityServer's `.AddConfigurationStore(...)`, which IdentityServer itself requires at runtime to look up clients/resources on every authorize/token request. Removing it would break the auth server. Both `.AddConfigurationStore()` (clients/resources) and `.AddOperationalStore()` (persisted grants) remain. `ApplicationDbContext` (ASP.NET Identity) and `DataProtectionDbContext` also remain.

**NuGet packages:** none were removed — every package in `Spydersoft.Identity.csproj` is still used by the remaining auth/identity code paths (Duende.IdentityServer.EntityFramework is still needed for the configuration/operational stores).

Verified: `dotnet build Spydersoft.Identity.slnx` → 0 errors (pre-existing style/analyzer warnings only). The `Spydersoft.Identity.Tests` project has no references to the removed admin code.

---

## OpenAPI Client ✅ COMPLETE (2026-05-27)

`yarn api:update` against Admin.Api on port 7030 produced:

- `admin-ui/openapi.json` (~212 KB)
- `admin-ui/src/api/generated/` — `client.gen.ts`, `core/`, `client/`, `sdk.gen.ts` (~1,856 lines), `types.gen.ts` (~3,336 lines), `index.ts` (~634 lines)

**Decision:** Both `openapi.json` and `src/api/generated/` are committed (not gitignored). Trade-off: noisier diffs on regen, but PRs show API surface changes explicitly and CI doesn't need Admin.Api running to build the SPA.

Regenerate with: `cd Spydersoft.Identity.Admin.Frontend/admin-ui && yarn api:update` (requires the stack running via AppHost so Admin.Api is reachable at `http://localhost:7030`).

---

## SPA shared infrastructure ✅ (2026-05-28)

- `src/api/client.ts` — axios instance with `withCredentials: true`, response interceptor that emits typed `ApiError` events and auto-redirects 401s to `/.auth/login?returnUrl=…`. Injected into the generated `@hey-api/client-axios` via `client.setConfig({ axios })` so all SDK calls share the interceptor.
- `src/components/PageHeader.tsx` — title + subtitle + icon + actions slot (eyebrow text, optional icon chip, actions on the right, bottom divider).
- `src/components/EmptyState.tsx` — icon + title + description + optional action, used for stub pages and "no items" states.
- `src/components/GlobalToast.tsx` — subscribes to `onApiError` and surfaces non-auth errors via PrimeReact `Toast`. Mounted once at the App root.
- `src/components/SubResourceList.tsx` — reusable list-with-add-row component for client/resource/scope sub-resources. Generic over the row type and create-draft type.
- `src/utils/format.ts` — `formatDate`, `toNumberOrUndefined`, `asId`.
- `main.tsx` calls `configureApi()` before React mounts.

## Styling architecture ✅ (2026-05-28)

The SPA follows the Pitstop pattern: **stock PrimeReact theme + Tailwind utilities, with minimal custom CSS**. Earlier attempts at heavy per-component CSS overrides led to whack-a-mole regressions (border-width, padding, cascade ordering) — now reverted.

- **Theme:** `primereact/resources/themes/lara-light-indigo/theme.css` (indigo brand). Imported via `@import` *inside* `styles.css`, **after** `@import "tailwindcss"`.
- **Cascade layer ordering (critical gotcha):** PrimeReact v10 lara themes wrap every rule in `@layer primereact { ... }`. CSS cascade-layer priority is fixed by **first appearance** in the bundle. If the theme is JS-imported in `main.tsx` before `styles.css`, `primereact` ends up declared first → lowest priority → Tailwind preflight (`@layer base`) overrides PrimeReact button/input/dropdown styling despite higher specificity (layers trump specificity). The fix is to `@import "tailwindcss"` **before** the theme `@import` inside `styles.css`. Compiled order should be: `properties → theme → base → components → utilities → primereact`. Pitstop has the same setup.
- **Tailwind utility wins over PrimeReact:** when a utility class needs to beat a layered PrimeReact rule (e.g. `pl-8` on a search input where PrimeReact's `.p-inputtext { padding: 0.75rem }` is in the higher-priority `primereact` layer), use the `!` suffix: `pl-8!` → emits `padding-left: 2rem !important`. `!important` reverses layer priority.
- **`@tailwindcss/forms`:** loaded with `strategy: class` (`@plugin "@tailwindcss/forms" { strategy: class; }`). Default `base` strategy styles raw `<input type="checkbox">` globally, causing PrimeReact's `InputSwitch` to render with both a blue checkbox AND a slider (visible dupe). Class strategy makes the plugin opt-in.
- **No PrimeIcons:** removed `primeicons.css` import and any `pi pi-*` usages (ConfirmDialog icons dropped). FontAwesome 7 covers all icon needs via `@fortawesome/react-fontawesome`.
- **`primereact.min.css`:** deprecated no-op in v10, not imported.
- **Custom CSS in `styles.css`:** only `@theme` tokens (brand/surface/content/border colors, font stacks) + body baseline + 4-property `.p-card` polish. Mirror of Pitstop's shape.
- **Brand tokens via `@theme`:** `--color-brand`, `--color-surface`, `--color-content-muted`, etc. — consumed by our custom React components as Tailwind utilities (`bg-brand`, `text-content-muted`, `border-border`). PrimeReact components use lara's own tokens (`--primary-color` etc.) and are not retargeted.
- **App shell:** redesigned `App.tsx` with sidebar (logo lockup + grouped nav sections + user chip with sign-out at bottom) and max-width content column with vertical scroll. `ClientEdit` has a sticky save bar (`fixed left-60 bottom-0`) aligned to the content column.

## Pages

| Page | Status | Notes |
|---|---|---|
| Clients | ✅ 2026-05-28 | List (DataTable + search), Edit (Settings tab + 9 sub-resource tabs: Scopes, Grant types, Redirect URIs, Post-logout URIs, CORS origins, Claims, Secrets, Properties, IdP restrictions). New-client form hides sub-resource tabs until the client exists. Visuals reworked 2026-05-28 (see Styling architecture). |
| ApiResources | ✅ 2026-05-28 | List (DataTable + search), Edit (Settings tab + 4 sub-resource tabs: Scopes, User claims, Secrets, Properties). Same shape as Clients. |
| IdentityResources | ✅ 2026-05-28 | List + Edit (Settings tab with Basic info + Consent behaviour sections) + 2 sub-resource tabs: User claims, Properties. |
| Scopes | ✅ 2026-05-28 | List + Edit (Settings tab with Basic info + Consent behaviour sections) + 2 sub-resource tabs: User claims, Properties. |
| Users | ✅ 2026-05-28 | List (DataTable + search), Edit (Settings tab w/ Account + Security sections; password field only on `isNew`) + Roles + Claims tabs. Roles tab uses a PrimeReact `Dropdown` populated from `GET /api/v1/roles`. Claims tab is read-only. GUID ids (string), not int. |
| Roles | ✅ 2026-05-28 | List + Edit (Settings tab with Name only) + Claims tab. Role claims keyed by claim *type* (not numeric id); custom panel mirrors the user RolesPanel shape since `SubResourceList`'s `Number(row.id)` delete signature does not fit. |

---

## Users / Roles — implementation notes

Users does **not** fit the resource-page template (`<Area>List` + `<Area>Edit` + `<Area>SubResources`) cleanly. Read this section before starting.

### Differences from resource pages

- **GUID ids, not int.** `UserSummaryDto.id` and `UserDto.id` are `string` (ASP.NET Core Identity GUIDs). All `path: { id }` params in the User SDK calls take strings — do **not** wrap with `Number()` the way the resource pages do.
- **Two create/edit DTOs.** `CreateUserDto` (POST `/users`) requires `password` + `userName` + `email`; `SaveUserDto` (PUT `/users/{id}`) has the same shape **minus password**. The Edit page needs to render a password field only on the "new" path, similar to how `ClientEdit` hides sub-resource tabs when `isNew`.
- **Read-only claims sub-resource.** `getApiV1UsersByIdClaims` exists, but no POST/PUT/DELETE. Render `UserClaimDto[]` as a plain table — no add row, no delete column. `SubResourceList` won't fit; build a small read-only table instead, or pass an `emptyCreateForm` that renders nothing.
- **Roles use role NAME as the path segment, not an id.** `deleteApiV1UsersByIdRolesByRoleName` takes `{ path: { id, roleName } }`. POST takes `{ body: AssignUserRoleDto }` which is `{ roleName: string }`. We went with option (b) and added a full `RolesController` (CRUD + claims). The User RolesPanel uses `getApiV1Roles()` to populate a `Dropdown` filtered to roles the user does not already have.
- **Role claims keyed by type, not id.** `deleteApiV1RolesByIdClaimsByClaimType` takes `{ path: { id, claimType } }` — the `RoleClaimDto` has only `{ type, value }`, no numeric id. `SubResourceList` cannot be used (it calls `remove(Number(row.id))`). The `roles/RoleSubResources.tsx` `ClaimsPanel` is a hand-rolled DataTable + add form instead.

### Relevant generated symbols

SDK functions ([src/api/generated/sdk.gen.ts](Spydersoft.Identity.Admin.Frontend/admin-ui/src/api/generated/sdk.gen.ts)):

- `getApiV1Users`, `postApiV1Users`, `getApiV1UsersById`, `putApiV1UsersById`, `deleteApiV1UsersById`
- `getApiV1UsersByIdRoles`, `postApiV1UsersByIdRoles`, `deleteApiV1UsersByIdRolesByRoleName`
- `getApiV1UsersByIdClaims`

DTOs ([src/api/generated/types.gen.ts](Spydersoft.Identity.Admin.Frontend/admin-ui/src/api/generated/types.gen.ts)):

- `UserSummaryDto` — `{ id, userName, email, emailConfirmed, name, twoFactorEnabled, lockoutEnabled }`
- `UserDto` — summary + `phoneNumber`, `phoneNumberConfirmed`, `accessFailedCount`
- `CreateUserDto` — `{ password, userName, email, name?, phoneNumber?, twoFactorEnabled?, lockoutEnabled? }`
- `SaveUserDto` — `CreateUserDto` minus `password`
- `UserRoleDto` — `{ roleName }` (returned by GET roles)
- `AssignUserRoleDto` — `{ roleName }` (POST body)
- `UserClaimDto` — `{ type, value }` (read-only)

### File layout

```text
src/pages/users/
  UsersList.tsx          — DataTable + search; columns: userName, email, name, status badges (email confirmed, 2FA, lockout)
  UserEdit.tsx           — TabView: Settings (Account + Security; password field on isNew only) + Roles + Claims tabs
  UserSubResources.tsx   — RolesPanel (Dropdown sourced from /api/v1/roles, add/remove by name), ClaimsPanel (read-only list)
src/pages/Users.tsx      — Routes wrapper

src/pages/roles/
  RolesList.tsx          — DataTable + search; columns: name
  RoleEdit.tsx           — TabView: Settings (name only) + Claims tab
  RoleSubResources.tsx   — ClaimsPanel (hand-rolled — delete keyed by claim type)
src/pages/Roles.tsx      — Routes wrapper
```

`App.tsx` has Users + Roles routes and both nav items under the People group.

---

## Test suites ✅ (2026-05-28 / 2026-05-29)

Two Playwright suites at `identity_server/tests/`. Both are CI-runnable — they bring the whole stack up via Playwright's `webServer` config, no Visual Studio dependency.

### `admin-api-integration/` — REST API tests (22 specs, ~35s)

- **Stack:** Playwright's `webServer` runs `dotnet run --project Spydersoft.Identity.AppHost --launch-profile Testing`. globalSetup waits for `/.well-known/openid-configuration`, then fetches a token via `client_credentials` from the seeded `identity.admin.tests` client. A fixture replaces the default `request` fixture with one that carries the bearer token.
- **Specs:** `clients`, `apiresources`, `identityresources`, `scopes`, `users`, `roles` — each covers list / get / create / update / delete + a sub-resource sample.
- **Files:** `package.json`, `tsconfig.json`, `playwright.config.ts`, `globalSetup.ts`, `fixtures.ts`, `tests/types.ts`, `tests/*.spec.ts`.

### `admin-ui-e2e/` — Browser tests (9 specs, ~37s)

- **Stack:** Two-entry `webServer` — (1) the AppHost in Testing profile (everything HTTP, no Vite, no HTTPS endpoint on the BFF); (2) `yarn dev` with `VITE_DEV_HTTP=1` for the Vite dev server. A `setup` project drives the real OIDC login flow once and saves `storageState`; the `chromium` project depends on `setup` and reuses the cookies for every spec.
- **Specs:**
  - `navigation.spec.ts` — sidebar + heading + seeded-data assertions across all six section pages.
  - `roles-crud.spec.ts` — full create → list → delete UI walkthrough with the PrimeReact `ConfirmDialog`.
- **Files:** `package.json`, `tsconfig.json`, `playwright.config.ts`, `tests/auth.setup.ts`, `tests/navigation.spec.ts`, `tests/roles-crud.spec.ts`.

### Testing-mode config notes

The Testing launch profile (`ASPNETCORE_ENVIRONMENT=Testing`) deliberately diverges from dev in a few places. These divergences are intentional and live in `Spydersoft.Identity.AppHost/Program.cs`:

- **Ephemeral Postgres** — `WithDataVolume` is skipped, so each test run starts from a clean DB and the seeder repopulates everything. Integration tests are hermetic across runs.
- **No Vite from Aspire** — `AddViteApp` is skipped because Aspire's `WithYarn` does not reliably spawn yarn from a non-interactive bash-launched dotnet host. The `admin-ui-e2e` suite runs `yarn dev` itself.
- **BFF HTTP only** — the admin-frontend HTTPS endpoint (7041) is dropped; only HTTP (7040) is bound. Kestrel's dev-cert pickup is flaky from a bash-spawned dotnet, even with the cert trusted in the Windows store. The seeded `identity.admin.frontend` client therefore registers four redirect URIs (HTTPS+HTTP × BFF-direct + Vite-host) so the same client works in both dev and testing.
- **`OidcProxySettings:AlwaysRedirectToHttps=false`** — OidcProxy.Net's `RedirectUriFactory` force-upgrades any `redirect_uri` to HTTPS by default. We explicitly disable that under Testing so the OIDC callback stays HTTP.

The test-only client `identity.admin.tests` (committed cleartext secret in `Spydersoft.Identity.DataSeeder/Seeding/Clients.cs`) is the `client_credentials` grant used by the API integration suite. It's seeded into every environment — present but unused outside the test project.

### Running locally

```text
# API integration tests (22, ~35s) — boots full stack via Playwright webServer
cd identity_server/tests/admin-api-integration && npx playwright test

# UI E2E tests (9, ~37s) — boots stack + yarn dev via Playwright webServer
cd identity_server/tests/admin-ui-e2e && npx playwright test
```

`reuseExistingServer: true` (when not in CI) means a stack already running via VS will be reused instead of double-launched.

---

## Next Immediate Actions

1. **Cross-app visual consistency (optional, deferred).** The admin SPA uses PrimeReact `lara-light-indigo` (indigo brand) with brand tokens defined in `@theme`. The main app uses Tailwind v4 + DaisyUI v5 with an OKLCH "identity" theme (`Spydersoft.Identity/Styles/style.css`). Different libraries, different visual languages — full unification would require either bringing DaisyUI into the admin SPA (and dropping PrimeReact) or vice versa, neither of which is needed for functional parity. Brand color hex codes can be aligned cosmetically if desired.

2. **Phase 3 cleanup** — strip the old MVC admin (Clients, ApiResources, IdentityResources, Scopes, Users, UserRoles controllers + views) from `Spydersoft.Identity` now that all pages have parity in the SPA and the e2e suite proves the flow end-to-end.

3. **Expand the e2e coverage** as needed — currently we have one CRUD walkthrough (Roles); the same pattern can extend to Users password-on-create, Clients sub-resource tabs, etc.

4. **Wire both test suites into CI** — they expect the Aspire/Podman/DotNet 10 toolchain to be available on the runner; no other external state required.
