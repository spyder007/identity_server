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
        new ApiScope("identity:read", "Identity API") { Description = "Read access to Identity API" },
        new ApiScope("identity:write", "Identity API") { Description = "Write access to Identity API" }
    };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("identity.api", "Identity API")
        {
            Scopes = { "identity:read", "identity:write" },
            ShowInDiscoveryDocument = false,
        }
    };
}
