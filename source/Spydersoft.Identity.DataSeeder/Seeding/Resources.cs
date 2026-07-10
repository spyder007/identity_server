using Duende.IdentityModel;

using Duende.IdentityServer.Models;

namespace Spydersoft.Identity.DataSeeder.Seeding;

internal static class Resources
{
    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResources.Address(),
        new IdentityResources.Phone(),
        new("roles", "User Roles", new[] { JwtClaimTypes.Role }),
    };

    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope("identity:admin:read", "Identity Admin API") { Description = "Read access to Identity Admin API" },
        new ApiScope("identity:admin:write", "Identity Admin API") { Description = "Write access to Identity Admin API" }
    };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("identity.admin.api", "Identity Admin API")
        {
            Scopes = { "identity:admin:read", "identity:admin:write" },
            ShowInDiscoveryDocument = false,
            UserClaims = { JwtClaimTypes.Role },
        }
    };
}
