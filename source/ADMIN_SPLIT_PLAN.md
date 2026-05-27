# Admin Split Plan — `feature/admin-split`

> **Saved:** 2025-08-20  
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
- **Scopes:** `identity:admin:read` (GET) and `identity:admin:write` (POST/PUT/DELETE)
- **API versioning:** URL segment — `/api/v1/...`
- **OpenAPI:** `Microsoft.AspNetCore.OpenApi` + Scalar UI (dev only), spec for Kiota client gen
- **Frontend:** React SPA hosted in an ASP.NET Core BFF project (`Admin.Frontend`); BFF proxies to Admin.Api — may have its own API endpoints as needed
- **Kiota:** Generate typed Admin.Api client from OpenAPI spec for use in Admin.Frontend
- **Phased approach:** Old MVC admin controllers/views in `Spydersoft.Identity` stay **untouched** until Admin.Frontend reaches full parity

---

## Existing Projects (unchanged)

| Project | Role |
|---|---|
| `Spydersoft.Identity` | Monolith — auth + old MVC admin (untouched in Phase 1/2) |
| `Spydersoft.Identity.Core` | Shared models, ViewModels, exceptions, service interfaces |
| `Spydersoft.Identity.Data` | EF DbContexts (`ApplicationDbContext`, Duende config/persisted grant), migrations |
| `Spydersoft.Identity.Tests` | NUnit test project |

---

## Phase 1 — `Spydersoft.Identity.Admin.Api` ✅ COMPLETE

### What was built

- **Project:** `Spydersoft.Identity.Admin.Api.csproj` — `net10.0`, CPM, references `Core` + `Data`
- **Packages added to `Directory.Packages.props`:** `Asp.Versioning.Http`, `Asp.Versioning.Mvc.ApiExplorer`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`
- **`Program.cs`:** EF (`ConfigurationDbContext` + `ApplicationDbContext`), ASP.NET Core Identity (UserManager/RoleManager), AutoMapper, JwtBearer auth, read/write scope policies, API versioning, OpenAPI + Scalar
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
```

### Config

- `appsettings.json` — keys: `ConnectionStrings:IdentityConnection`, `IdentityServer:Authority`, `AutoMapper:License`, `Telemetry`, `HealthChecks`
- `appsettings.Development.json` — debug-level Serilog
- `Properties/launchSettings.json` — port `34150`, launchUrl `scalar/v1`

---

## Phase 2 — `Spydersoft.Identity.Admin.Frontend` ✅ SCAFFOLD COMPLETE

### Architecture decisions made

- **BFF:** OidcProxy.Net.OpenIdConnect 5.4.1 (YARP-based) — exact mirror of `Spydersoft.PitStop.Frontend` pattern
- **SPA:** React 19 + Vite 8 + TypeScript in `admin-ui/` subfolder; linked as `.esproj`
- **UI components:** PrimeReact + Tailwind v4 (same as pitstop-ui)
- **API client gen:** `@hey-api/openapi-ts` + `@hey-api/client-axios` (same as pitstop-ui, not Kiota)
- **Routing:** React Router v7 (`react-router-dom`)
- **Token strategy:** OidcProxy.Net handles cookie↔Bearer exchange entirely server-side

### What was built

| File | Purpose |
|---|---|
| `Spydersoft.Identity.Admin.Frontend.csproj` | ASP.NET Core host, SpaRoot=admin-ui, port 9082/9083 |
| `Program.cs` | AddOidcProxy + UseOidcProxy, static files, MapFallbackToFile index.html |
| `appsettings.json` | OidcProxySettings: OIDC authority/clientId/scopes + YARP /api/** → adminApi @ localhost:34150 |
| `appsettings.Development.json` | Debug-level Serilog overrides |
| `Properties/launchSettings.json` | http:9082 / https:9083 |
| `admin-ui/admin-ui.esproj` | JavaScript SDK project (yarn dev / yarn build) |
| `admin-ui/package.json` | React 19, Vite 8, PrimeReact, Tailwind, @hey-api, React Router 7, vitest |
| `admin-ui/vite.config.mts` | Proxy ^/api + ^/.auth → https://localhost:9083, dev port 5210, dotnet dev-certs |
| `admin-ui/tsconfig.json` + `tsconfig.node.json` | TypeScript strict config |
| `admin-ui/index.html` | Entry HTML with /config.js runtime config script |
| `admin-ui/src/styles.css` | Tailwind v4 + PrimeReact theme + blue brand palette |
| `admin-ui/src/main.tsx` | React root with BrowserRouter + PrimeReactProvider |
| `admin-ui/src/App.tsx` | Sidebar nav layout + React Router routes to 5 admin sections |
| `admin-ui/openapi-ts.config.ts` | @hey-api config: input openapi.json → src/api/generated |
| `admin-ui/src/pages/Clients.tsx` | Stub |
| `admin-ui/src/pages/ApiResources.tsx` | Stub |
| `admin-ui/src/pages/IdentityResources.tsx` | Stub |
| `admin-ui/src/pages/Scopes.tsx` | Stub |
| `admin-ui/src/pages/Users.tsx` | Stub |
| `admin-ui/public/config.js` | Runtime config: `window.APP_CONFIG = { apiBasePath: "/api" }` |

### Ports

| Service | Port |
|---|---|
| Admin.Api | http://localhost:34150 |
| Admin.Frontend BFF (http) | http://localhost:9082 |
| Admin.Frontend BFF (https) | https://localhost:9083 |
| Vite dev server | https://localhost:5210 |

---

## Phase 3 — Strip old MVC Admin from `Spydersoft.Identity` 🔲 TODO

Once `Admin.Frontend` is at full parity:

- Remove `Controllers/Admin/*` (Clients, ApiResources, IdentityResources, Scopes)
- Remove `Controllers/UserAdmin/*` (Users, UserRoles)
- Remove corresponding Views
- Remove `ConfigurationDbContext` registration from `Spydersoft.Identity` `Program.cs`
- Clean up unused using statements and NuGet packages

---

## Next Immediate Actions

1. **Register `identity.admin.frontend` client** in `Spydersoft.Identity` with:
   - `grant_type`: `authorization_code` + PKCE
   - `redirect_uri`: `https://localhost:9083/oidc/login/callback`
   - `post_logout_redirect_uri`: `https://localhost:9083/oidc/logout`
   - Allowed scopes: `openid profile email identity:admin:read identity:admin:write`

2. **Set user secrets for Admin.Frontend** (`UserSecretsId: b2c3d4e5-f6a7-8901-bcde-f12345678901`):
   - `OidcProxySettings:Oidc:ClientId` = `identity.admin.frontend`
   - `OidcProxySettings:Oidc:ClientSecret` = `<generated>`

3. **Set user secrets for Admin.Api:**
   - `ConnectionStrings:IdentityConnection` — same Postgres connection string as main app
   - `IdentityServer:Authority` — `http://localhost:34147` (main app local port)
   - `AutoMapper:License` — same license key

4. **Install JS deps:** `cd Spydersoft.Identity.Admin.Frontend/admin-ui && yarn`

5. **Snapshot OpenAPI spec** (with Admin.Api running at port 34150): `yarn api:spec && yarn api:generate`

6. **Implement pages:** Clients → ApiResources → IdentityResources → Scopes → Users

7. **Once parity → Phase 3:** strip old MVC admin from `Spydersoft.Identity`
