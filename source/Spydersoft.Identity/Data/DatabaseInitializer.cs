﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Exceptions;
using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Data
{
    /// <summary>
    /// Class DatabaseInitializer.
    /// </summary>
    public class DatabaseInitializer(IApplicationBuilder app)
    {
        /// <summary>
        /// The application
        /// </summary>
        private readonly IApplicationBuilder _app = app;
        /// <summary>
        /// The log
        /// </summary>
        private ILogger _log;

        /// <summary>
        /// Initializes the database.
        /// </summary>
        public void InitializeDatabase()
        {
            using IServiceScope serviceScope = _app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            _log = serviceScope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();

            PerformDatabaseMigrations(serviceScope);
            SeedDatabase(serviceScope);
        }

        #region Database Migration Methods

        /// <summary>
        /// Performs the database migrations.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        private void PerformDatabaseMigrations(IServiceScope serviceScope)
        {
            Task persistedGrantTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>(), "ASP Net Grants Database");

            Task appDbTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), "ASP Net User Database ");

            Task configTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>(), "Identity Server 4 Configuration Database");

            Task dataProtectTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<DataProtectionDbContext>(),
                    "Data Protection Database");

            Task.WaitAll(dataProtectTask, persistedGrantTask, appDbTask, configTask);
        }

        /// <summary>
        /// Does the migration if needed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        private async Task DoMigrationIfNeeded(DbContext context, string databaseName)
        {
            _log.LogDebug("Checking {database} for pending migrations.", databaseName);
            var hasMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
            if (hasMigrations)
            {
                _log.LogInformation("Migrating {database}.", databaseName);
                await context.Database.MigrateAsync();
            }
        }

        #endregion Database Migration Methods

        #region Seeding Functions

        /// <summary>
        /// Seeds the database.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        private void SeedDatabase(IServiceScope serviceScope)
        {
            SeedIdentityServerConfigurationDatabase(serviceScope);
            SeedAspNetIdentityDatabase(serviceScope);
        }

        /// <summary>
        /// Seeds the identity server configuration database.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        private void SeedIdentityServerConfigurationDatabase(IServiceScope serviceScope)
        {
            ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                _log.LogInformation("No Clients Found. Creating sample Clients");
                foreach (Client client in GetClients())
                {
                    _ = context.Clients.Add(client.ToEntity());
                }

                _ = context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                _log.LogInformation("No Resources Found. Creating sample Resources");
                foreach (IdentityResource resource in GetIdentityResources())
                {
                    _ = context.IdentityResources.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                _log.LogInformation("No API Resources Found. Creating sample API Resources");
                foreach (ApiResource resource in GetApiResources())
                {
                    _ = context.ApiResources.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }
        }

        /// <summary>
        /// Seeds the ASP net identity database.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        /// <exception cref="IdentityResultException">result</exception>
        private void SeedAspNetIdentityDatabase(IServiceScope serviceScope)
        {
            RoleManager<ApplicationRole> roleMgr = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            ApplicationRole adminRole = roleMgr.FindByNameAsync("admin").Result;
            if (adminRole == null)
            {
                IdentityResult result = roleMgr.CreateAsync(new ApplicationRole()
                {
                    Name = "admin"
                }).Result;

                if (!result.Succeeded)
                {
                    _log.LogError("Unable to create admin role : {error}", result.ToString());
                }
            }


            UserManager<ApplicationUser> userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser adminUser = userMgr.FindByNameAsync("admin").Result;
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin"
                };
                IdentityResult result = userMgr.CreateAsync(adminUser, "Ch@ng3m3").Result;
                if (!result.Succeeded)
                {
                    throw new IdentityResultException(result);
                }

                adminUser = userMgr.FindByNameAsync("admin").Result;

                result = userMgr.AddClaimsAsync(adminUser, [
                                new(JwtClaimTypes.Name, "System Administrator"),
                    new(JwtClaimTypes.GivenName, "System"),
                    new(JwtClaimTypes.FamilyName, "administrator"),
                    new(JwtClaimTypes.Email, "admin@localhost.net"),
                    new(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new(JwtClaimTypes.WebSite, "123 NoWhere"),
                    new(JwtClaimTypes.Address, /*lang=json*/ @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                            ]).Result;
                if (!result.Succeeded)
                {
                    throw new IdentityResultException(result);
                }

                result = userMgr.AddToRoleAsync(adminUser, "admin").Result;
                if (!result.Succeeded)
                {
                    throw new IdentityResultException(result);
                }

                _log.LogInformation("admin created");
            }
            else
            {
                _log.LogDebug("admin already exists");
            }

        }

        #endregion Seeding Functions

        #region Identity Server Configuration Object Creators

        /// <summary>
        /// Gets the identity resources.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;Duende.IdentityServer.Models.IdentityResource&gt;.</returns>
        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            ];
        }

        /// <summary>
        /// Gets the API resources.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;Duende.IdentityServer.Models.ApiResource&gt;.</returns>
        private static IEnumerable<ApiResource> GetApiResources()
        {
            return
            [
                new("api", "My API")
            ];
        }

        // clients want to access resources (aka scopes)
        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;Duende.IdentityServer.Models.Client&gt;.</returns>
        private static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return
            [
                new()
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api" }
                },

                // resource owner password grant client
                new()
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api" }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new()
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api"
                    },
                    AllowOfflineAccess = true
                }
            ];
        }
        #endregion Identity Server Configuration Object Creators
    }
}