var builder = DistributedApplication.CreateBuilder(args);

// Optional user-secret: AutoMapper license key. Falls through to empty string;
// the apps treat empty as unlicensed (free-tier) and run fine for local dev.
var automapperLicense = builder.Configuration["AutoMapper:License"] ?? string.Empty;

// Cleartext OIDC client secret for identity.admin.frontend. Single source of
// truth: the seeder reads this, SHA-256-hashes it, and writes it as the DB
// client secret. The admin BFF reads the same value and presents it at the
// token endpoint. Override via `dotnet user-secrets set Parameters:admin-frontend-clientsecret <value>`
// on the AppHost or via the Aspire dashboard. Default "secret" matches the
// other dev clients in the seed.
var adminFrontendClientSecret = builder.AddParameter(
    "admin-frontend-clientsecret",
    "secret",
    secret: true);

// PostgreSQL with persistent named volume for normal dev runs (so reseeding
// is skipped on restart and pgAdmin/psql/DBeaver connections stay stable).
// Under the Testing launch profile we deliberately drop the volume so each
// test run starts from a clean DB and the seeder repopulates everything;
// integration tests must be hermetic across runs.
//
// The password MUST be a stable, explicitly-pinned parameter: with the
// persistent volume below, the password is baked into the data directory on
// first init and never re-read. If the password drifts between runs the
// connection string no longer matches the volume and every connection fails.
// The generated default is persisted to user-secrets (Parameters:postgres-password)
// so it stays constant; override via
// `dotnet user-secrets set Parameters:postgres-password <value>`.

IResourceBuilder<PostgresServerResource> postgres;

if (builder.Environment.EnvironmentName != "Testing")
{
    var postgresPassword = builder.AddParameter("postgres-password", secret: true);
    postgres = builder.AddPostgres("postgres", password: postgresPassword, port: 7010);
    postgres.WithDataVolume("identity-pg-data");
}
else
{
    postgres = builder.AddPostgres("postgres", port: 7010);
}

// Resource name "identitydb" to avoid collision with the "identity" project
// below; databaseName="identity" keeps the actual Postgres DB name unchanged.
var identityDb = postgres.AddDatabase("identitydb", "identity");

// Main IdentityServer + MVC admin (existing site). The Telemetry__*__Type
// overrides flip the dev config (which routes telemetry to console) back to
// OTLP so that under Aspire the identity app emits to the dashboard. The
// OTLP endpoint comes from the standard OTEL_EXPORTER_OTLP_ENDPOINT env var
// that Aspire injects automatically; matches admin-api/admin-frontend.
var identity = builder.AddProject<Projects.Spydersoft_Identity>("identity")
    .WithReference(identityDb)
    .WithEnvironment("ConnectionStrings__IdentityConnection", identityDb)
    .WithEnvironment("AutoMapper__License", automapperLicense)
    // Pin the announced issuer so it matches the URL admin-api uses as its
    // authority. Without this the token's `iss` is derived from the request
    // hostname (localhost vs 127.0.0.1) and the bearer pipeline rejects
    // tokens whose iss does not exactly match the discovery doc's issuer.
    .WithEnvironment("IdentityServer__IssuerUri", "http://localhost:7020")
    .WithEnvironment("Telemetry__Log__Type", "otlp")
    .WithEnvironment("Telemetry__Metrics__Type", "otlp")
    .WithEnvironment("Telemetry__Trace__Type", "otlp")
    .WithEndpoint("http", e => { e.Port = 7020; e.TargetPort = 7020; e.IsProxied = false; });

// One-shot seeder: applies EF migrations then seeds Clients/Resources/Identity.
// Under Testing it starts automatically and identity waits for it (clean DB on
// every run). In dev it is registered but not started — use the Aspire dashboard
// to trigger it manually on a fresh local instance.
var seeder = builder.AddProject<Projects.Spydersoft_Identity_DataSeeder>("seeder")
    .WithReference(identityDb)
    .WaitFor(identityDb)
    .WithEnvironment("ConnectionStrings__IdentityConnection", identityDb)
    .WithEnvironment("AdminFrontend__ClientSecret", adminFrontendClientSecret);

if (builder.Environment.EnvironmentName == "Testing")
{
    identity.WaitForCompletion(seeder);
}
else
{
    seeder.WithExplicitStart();
    identity.WaitFor(identityDb);
}

// Admin REST API.
var adminApi = builder.AddProject<Projects.Spydersoft_Identity_Admin_Api>("admin-api")
    .WithReference(identityDb)
    .WithEnvironment("ConnectionStrings__IdentityConnection", identityDb)
    .WithEnvironment("AutoMapper__License", automapperLicense)
    .WithEnvironment("IdentityServer__Authority", identity.GetEndpoint("http"))
    .WaitFor(identity)
    .WithEndpoint("http", e => { e.Port = 7030; e.TargetPort = 7030; e.IsProxied = false; });

// Admin BFF (OidcProxy.Net + YARP). Receives the same cleartext secret as the
// seeder so the token-endpoint authentication matches the DB-stored hash.
// Under Testing we expose http only — Kestrel's dev-cert pickup is flaky from
// a bash-spawned dotnet host, and CI doesn't need HTTPS on localhost. The
// seeded identity.admin.frontend client carries BOTH redirect URIs so the
// same client works in dev (https://localhost:7041) and Testing (http://localhost:7040).
var adminFrontend = builder.AddProject<Projects.Spydersoft_Identity_Admin_Frontend>("admin-frontend")
    .WithEnvironment("OidcProxySettings__Oidc__Authority", identity.GetEndpoint("http"))
    .WithEnvironment("OidcProxySettings__Oidc__ClientId", "identity.admin.frontend")
    .WithEnvironment("OidcProxySettings__Oidc__ClientSecret", adminFrontendClientSecret)
    .WithEnvironment(
        "OidcProxySettings__ReverseProxy__Clusters__adminApi__Destinations__destination1__Address",
        adminApi.GetEndpoint("http"))
    .WaitFor(identity)
    .WaitFor(adminApi)
    .WithEndpoint("http", e => { e.Port = 7040; e.TargetPort = 7040; e.IsProxied = false; });
if (builder.Environment.EnvironmentName != "Testing")
{
    adminFrontend.WithEndpoint("https", e => { e.Port = 7041; e.TargetPort = 7041; e.IsProxied = false; });
}
else
{
    // OidcProxy.Net's RedirectUriFactory force-upgrades the redirect_uri to
    // HTTPS by default (AlwaysRedirectToHttps defaults to true). The Testing
    // profile runs everything over HTTP, so we explicitly disable that.
    adminFrontend.WithEnvironment("OidcProxySettings__AlwaysRedirectToHttps", "false");
}

// Vite dev server for the React SPA. Yarn is configured via .yarnrc.yml in admin-ui.
// Skipped under ASPNETCORE_ENVIRONMENT=Testing because Aspire's WithYarn does
// not reliably spawn yarn under a non-interactive bash-launched dotnet host
// (works in VS, never starts from a bash subshell). The Playwright e2e suite
// runs `yarn dev` itself as a second webServer entry.
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.AddViteApp("admin-ui", "../Spydersoft.Identity.Admin.Frontend/admin-ui")
        .WithYarn()
        .WithEndpoint("http", e => { e.Port = 7050; e.IsProxied = false; e.UriScheme = "https"; });
}

await builder.Build().RunAsync();
