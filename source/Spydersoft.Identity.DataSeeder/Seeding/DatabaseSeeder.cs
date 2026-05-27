using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Data;

namespace Spydersoft.Identity.DataSeeder.Seeding;

internal static class DatabaseSeeder
{
    public static async Task RunAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<DatabaseSeederMarker>>();

        logger.LogInformation("Applying EF migrations.");
        await sp.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

        var configDb = sp.GetRequiredService<ConfigurationDbContext>();
        var appConfig = sp.GetRequiredService<IConfiguration>();

        var adminFrontendSecret = appConfig["AdminFrontend:ClientSecret"]
            ?? throw new InvalidOperationException(
                "AdminFrontend:ClientSecret is not configured. Under Aspire this is " +
                "supplied automatically via the AppHost parameter resource.");

        await SeedIdentityResourcesAsync(configDb, logger);
        await SeedApiScopesAsync(configDb, logger);
        await SeedApiResourcesAsync(configDb, logger);
        await SeedClientsAsync(configDb, adminFrontendSecret, logger);

        await Identity.SeedAsync(
            sp.GetRequiredService<RoleManager<ApplicationRole>>(),
            sp.GetRequiredService<UserManager<ApplicationUser>>());

        logger.LogInformation("Seed complete.");
    }

    private static async Task SeedIdentityResourcesAsync(ConfigurationDbContext db, ILogger logger)
    {
        var existing = await db.IdentityResources.Select(r => r.Name).ToListAsync();
        foreach (var resource in Resources.IdentityResources)
        {
            if (!existing.Contains(resource.Name))
            {
                logger.LogInformation("Adding identity resource {Name}", resource.Name);
                db.IdentityResources.Add(resource.ToEntity());
            }
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedApiScopesAsync(ConfigurationDbContext db, ILogger logger)
    {
        var existing = await db.ApiScopes.Select(s => s.Name).ToListAsync();
        foreach (var scope in Resources.ApiScopes)
        {
            if (!existing.Contains(scope.Name))
            {
                logger.LogInformation("Adding API scope {Name}", scope.Name);
                db.ApiScopes.Add(scope.ToEntity());
            }
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedApiResourcesAsync(ConfigurationDbContext db, ILogger logger)
    {
        var existing = await db.ApiResources.Select(r => r.Name).ToListAsync();
        foreach (var resource in Resources.ApiResources)
        {
            if (!existing.Contains(resource.Name))
            {
                logger.LogInformation("Adding API resource {Name}", resource.Name);
                db.ApiResources.Add(resource.ToEntity());
            }
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedClientsAsync(
        ConfigurationDbContext db,
        string adminFrontendClientSecret,
        ILogger logger)
    {
        var existing = await db.Clients.Select(c => c.ClientId).ToListAsync();
        foreach (var client in Clients.All(adminFrontendClientSecret))
        {
            if (!existing.Contains(client.ClientId))
            {
                logger.LogInformation("Adding client {ClientId}", client.ClientId);
                db.Clients.Add(client.ToEntity());
            }
        }
        await db.SaveChangesAsync();
    }

    // Marker type so ILogger<T> gets a useful category name.
    private sealed class DatabaseSeederMarker;
}
