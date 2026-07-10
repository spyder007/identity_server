# identity chart

OCI chart for Spydersoft Identity's three controllers: `main` (Duende IdentityServer), `admin-api` (admin REST API), and `admin-frontend` (OidcProxy.Net BFF + React admin UI). Published from this repo (`ghcr.io/spydersoft-consulting/charts/identity`), versioned alongside the container images.

This chart does **not** own or create any Kubernetes `Secret`, `ConfigMap`, or backing infrastructure (PostgreSQL). It only references config/secrets **by name**, via `envFrom.secretRef`/`envFrom.configMapRef`, with the names themselves overridable values. Whoever composes this chart (today: `platform-helm-config`) is responsible for creating the referenced Secret/ConfigMap and for owning the Postgres instance.

## Values

- `controllers.{main,admin-api,admin-frontend}.containers.main.image.tag` — per-controller image tag.
- `controllers.{main,admin-api,admin-frontend}.containers.main.envFrom` — supplied entirely by the caller; not defaulted here (every real caller overrides this in full to add its own `configMapRef`s — see the secrets contract below for what each secret must contain).
- `route.{main,admin-api,admin-frontend}.hostnames` — per-environment hostnames; not defaulted here since every real caller supplies them.
- `controllers.admin-frontend.containers.main.env` — **is** defaulted: sets `OidcProxySettings__ReverseProxy__Clusters__adminApi__Destinations__destination1__Address` to `http://{{ include "identity.fullname" . }}-admin-api/`, computed via the chart's own naming helper. This is genuinely stable across every caller — all three controllers always live in the same release/namespace — unlike the values above. Don't hardcode a literal service name here or in the caller's overlay; if you ever rename the release, this value moves with it automatically.

## Secrets contract

The caller must create a secret named **`identity-server-secrets`** (consumed by the `main` controller) containing:

- `ConnectionStrings__IdentityConnection` — Postgres connection string.
- `ConnectionStrings__RedisCache` — Redis connection string.
- `ProviderSettings__GoogleClientId` / `ProviderSettings__GoogleClientSecret`
- `Resend__ApiKey` / `Resend__EmailFromAddress`
- `Automapper__License`

The caller must create a secret named **`identity-server-admin-api-secrets`** (consumed by `admin-api`) containing:

- `ConnectionStrings__IdentityConnection` — same Postgres database as `main`.
- `AutoMapper__License`

The caller must create a secret named **`identity-server-admin-ui-secrets`** (consumed by `admin-frontend`) containing:

- `OidcProxySettings__Oidc__ClientSecret`

None of these secret names are hardcoded in this chart — all three are supplied via each controller's `envFrom.secretRef.name` override, so a different composing repo could name/source them however it wants without any chart change.
