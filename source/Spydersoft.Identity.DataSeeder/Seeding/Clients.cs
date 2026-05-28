using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Spydersoft.Identity.DataSeeder.Seeding;

internal static class Clients
{
    // Pre-hashed shared secret values lifted verbatim from the source DB so
    // existing client credentials keep working without rotation.
    private const string Secret_K7gNU3 = "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=";
    private const string Secret_QzotTq = "QzotTqTf4cOhHd5ssas1diDM+TuNUWAfpBxi+GLfCAY=";
    private const string Secret_WZzpb9 = "WZzpb954PpwysWAv7HE8t+8Et1VnRBQ5L6nFlC+RPOE=";

    // Cleartext secret for the `identity.admin.tests` client_credentials client.
    // Committed deliberately — local-only test fixture; the client is seeded
    // in dev for the Playwright suite to fetch admin-scope tokens against the
    // real /connect/token endpoint. Do not reuse outside the tests project.
    public const string AdminTestsClientSecret = "local-tests-only-do-not-deploy";

    private static Secret Hashed(string hash) => new(hash) { Type = IdentityServerConstants.SecretTypes.SharedSecret };

    /// <param name="adminFrontendClientSecret">
    /// Cleartext OIDC client secret for `identity.admin.frontend`. Supplied by
    /// the AppHost (Aspire parameter) so the same value is also injected into
    /// the BFF's `OidcProxySettings:Oidc:ClientSecret`. Hashed via SHA-256
    /// before being persisted.
    /// </param>
    public static IEnumerable<Client> All(string adminFrontendClientSecret) => new[]
    {
        new Client
        {
            ClientId = "client",
            AllowedGrantTypes = { GrantType.AuthorizationCode, GrantType.ClientCredentials },
            RequireConsent = true,
            AllowRememberConsent = false,
            ClientSecrets = { Hashed(Secret_K7gNU3) },
            RedirectUris = { "https://developer.mattgerega.com" },
            AllowedScopes = { "email", "hbn.api", "identity.api", "openid", "profile" },
            AllowOfflineAccess = true,
        },
        new Client
        {
            ClientId = "ro.client",
            AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
            RequireConsent = true,
            AllowRememberConsent = false,
            ClientSecrets = { Hashed(Secret_K7gNU3), Hashed(Secret_WZzpb9) },
            AllowedScopes = { "hbn.api", "identity.api", "openid", "profile", "unifi.ipmanager" },
            AllowOfflineAccess = true,
        },
        new Client
        {
            ClientId = "mvc",
            ClientName = "MVC Client",
            AllowedGrantTypes = { GrantType.Hybrid, GrantType.ClientCredentials },
            RequireConsent = true,
            ClientSecrets = { Hashed(Secret_K7gNU3) },
            RedirectUris = { "http://localhost:5002/signin-oidc", "http://localhost:52681" },
            PostLogoutRedirectUris = { "http://localhost:5002" },
            AllowedScopes = { "hbn.api", "identity.api", "openid", "profile" },
            AllowOfflineAccess = true,
        },
        new Client
        {
            ClientId = "postman.client",
            ClientName = "Postman",
            AllowedGrantTypes = { GrantType.AuthorizationCode },
            AllowAccessTokensViaBrowser = true,
            AlwaysIncludeUserClaimsInIdToken = true,
            ClientSecrets = { Hashed(Secret_QzotTq) },
            RedirectUris = { "http://localhost:1234" },
            AllowedScopes =
            {
                "email", "ha.api.isywrapper", "ha.kiosk.api", "hbn.api",
                "hbn.api.beers", "openid", "profile", "unifi.ipmanager",
            },
            Claims = { new ClientClaim("test", "ffdfd") },
            AllowOfflineAccess = true,
        },
        // BFF for the Admin SPA. Authenticates via authorization-code + PKCE;
        // tokens stay server-side (AllowAccessTokensViaBrowser = false). The
        // cleartext secret is provided by the AppHost (Aspire parameter) so
        // it matches what the BFF presents at the token endpoint.
        new Client
        {
            ClientId = "identity.admin.frontend",
            ClientName = "Identity Admin Frontend",
            AllowedGrantTypes = { GrantType.AuthorizationCode },
            RequirePkce = true,
            RequireConsent = false,
            AlwaysIncludeUserClaimsInIdToken = true,
            ClientSecrets = { Hashed(adminFrontendClientSecret.Sha256()) },
            RedirectUris = { "https://localhost:7041/oidc/login/callback" },
            PostLogoutRedirectUris = { "https://localhost:7041/oidc/logout" },
            AllowedScopes =
            {
                "openid", "profile", "email",
                "identity:admin:read", "identity:admin:write",
            },
            AllowOfflineAccess = true,
        },
        // Client_credentials client used by the Playwright admin-api-integration
        // suite to fetch a token with the admin scopes. Secret is committed in
        // source — see AdminTestsClientSecret above.
        new Client
        {
            ClientId = "identity.admin.tests",
            ClientName = "Identity Admin Tests",
            AllowedGrantTypes = { GrantType.ClientCredentials },
            ClientSecrets = { Hashed(AdminTestsClientSecret.Sha256()) },
            AllowedScopes = { "identity:admin:read", "identity:admin:write" },
        },
    };
}
