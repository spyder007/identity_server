using System.Security.Claims;

using Duende.IdentityModel;
using Duende.IdentityServer;

using Microsoft.AspNetCore.Identity;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.DataSeeder.Seeding;

internal static class Identity
{
    public static readonly string[] RoleNames = ["authentication.admin", "developer"];

    public static async Task SeedAsync(
        RoleManager<ApplicationRole> roleMgr,
        UserManager<ApplicationUser> userMgr)
    {
        foreach (var role in RoleNames)
        {
            if (await roleMgr.FindByNameAsync(role) is null)
            {
                var result = await roleMgr.CreateAsync(new ApplicationRole { Name = role });
                if (!result.Succeeded)
                {
                    throw new IdentityResultException(result);
                }
            }
        }

        var admin = await userMgr.FindByNameAsync("admin");
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@localhost.net",
            };

            var create = await userMgr.CreateAsync(admin, "Ch@ng3m3");
            if (!create.Succeeded)
            {
                throw new IdentityResultException(create);
            }

            admin = await userMgr.FindByNameAsync("admin");

            var claims = await userMgr.AddClaimsAsync(admin!, [
                new Claim(JwtClaimTypes.Name, "System Administrator"),
                new Claim(JwtClaimTypes.GivenName, "System"),
                new Claim(JwtClaimTypes.FamilyName, "administrator"),
                new Claim(JwtClaimTypes.Email, "admin@localhost.net"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.WebSite, "123 NoWhere"),
                new Claim(
                    JwtClaimTypes.Address,
                    /*lang=json*/ "{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                    IdentityServerConstants.ClaimValueTypes.Json),
            ]);
            if (!claims.Succeeded)
            {
                throw new IdentityResultException(claims);
            }
        }

        if (!await userMgr.IsInRoleAsync(admin!, "authentication.admin"))
        {
            var roleAssign = await userMgr.AddToRoleAsync(admin!, "authentication.admin");
            if (!roleAssign.Succeeded)
            {
                throw new IdentityResultException(roleAssign);
            }
        }
    }
}
