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

// PostgreSQL with persistent named volume. Port pinned for stable host-side
// tooling (pgAdmin/psql/DBeaver) connections across AppHost restarts.
var postgres = builder.AddPostgres("postgres", port: 7010)
    .WithDataVolume("identity-pg-data");

// Resource name "identitydb" to avoid collision with the "identity" project
// below; databaseName="identity" keeps the actual Postgres DB name unchanged.
var identityDb = postgres.AddDatabase("identitydb", "identity");

// One-shot seeder: applies EF migrations, then seeds Clients/Resources/Identity
// if not already present. Receives the cleartext admin-frontend secret and
// hashes it before storing.
var seeder = builder.AddProject<Projects.Spydersoft_Identity_DataSeeder>("seeder")
    .WithReference(identityDb)
    .WaitFor(identityDb)
    .WithEnvironment("ConnectionStrings__IdentityConnection", identityDb)
    .WithEnvironment("AdminFrontend__ClientSecret", adminFrontendClientSecret);

// Main IdentityServer + MVC admin (existing site).
var identity = builder.AddProject<Projects.Spydersoft_Identity>("identity")
    .WithReference(identityDb)
    .WithEnvironment("ConnectionStrings__IdentityConnection", identityDb)
    .WithEnvironment("AutoMapper__License", automapperLicense)
    .WaitForCompletion(seeder)
    .WithEndpoint("http", e => { e.Port = 7020; e.TargetPort = 7020; e.IsProxied = false; });

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
builder.AddProject<Projects.Spydersoft_Identity_Admin_Frontend>("admin-frontend")
    .WithEnvironment("OidcProxySettings__Oidc__Authority", identity.GetEndpoint("http"))
    .WithEnvironment("OidcProxySettings__Oidc__ClientId", "identity.admin.frontend")
    .WithEnvironment("OidcProxySettings__Oidc__ClientSecret", adminFrontendClientSecret)
    .WithEnvironment(
        "OidcProxySettings__ReverseProxy__Clusters__adminApi__Destinations__destination1__Address",
        adminApi.GetEndpoint("http"))
    .WaitFor(identity)
    .WaitFor(adminApi)
    .WithEndpoint("http", e => { e.Port = 7040; e.TargetPort = 7040; e.IsProxied = false; })
    .WithEndpoint("https", e => { e.Port = 7041; e.TargetPort = 7041; e.IsProxied = false; });

// Vite dev server for the React SPA. Yarn is configured via .yarnrc.yml in admin-ui.
builder.AddViteApp("admin-ui", "../Spydersoft.Identity.Admin.Frontend/admin-ui")
    .WithYarn()
    .WithEndpoint("http", e => { e.Port = 7050; e.IsProxied = false; e.UriScheme = "https"; });

await builder.Build().RunAsync();
