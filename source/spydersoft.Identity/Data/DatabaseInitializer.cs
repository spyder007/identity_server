﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

using IdentityModel;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using spydersoft.Identity.Exceptions;
using spydersoft.Identity.Models.Identity;

namespace spydersoft.Identity.Data
{
    public class DatabaseInitializer
    {
        private readonly IApplicationBuilder _app;
        private ILogger _log;

        public DatabaseInitializer(IApplicationBuilder app)
        {
            _app = app;
        }

        public void InitializeDatabase()
        {
            using IServiceScope serviceScope = _app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            _log = serviceScope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();

            PerformDatabaseMigrations(serviceScope);
            SeedDatabase(serviceScope);
        }

        #region Database Migration Methods

        private void PerformDatabaseMigrations(IServiceScope serviceScope)
        {
            Task persistedGrantTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>(), "ASP Net Grants Database");

            Task.WaitAll(persistedGrantTask);

            Task appDbTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), "ASP Net User Database ");

            Task.WaitAll(appDbTask);

            Task configTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>(), "Identity Server 4 Configuration Database");

            Task.WaitAll(configTask);

            Task dataProtectTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<DataProtectionDbContext>(),
                    "Data Protection Database");

            Task.WaitAll(dataProtectTask);
        }

        private async Task DoMigrationIfNeeded(DbContext context, string databaseName)
        {
            _log.LogDebug("Checking {0} for pending migrations.", databaseName);
            var hasMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
            if (hasMigrations)
            {
                _log.LogInformation("Migrating {0}.", databaseName);
                await context.Database.MigrateAsync();
            }
        }

        #endregion Database Migration Methods

        #region Seeding Functions

        private void SeedDatabase(IServiceScope serviceScope)
        {
            SeedIdentityServerConfigurationDatabase(serviceScope);
            SeedAspNetIdentityDatabase(serviceScope);
        }

        private void SeedIdentityServerConfigurationDatabase(IServiceScope serviceScope)
        {
            ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                foreach (Client client in GetClients())
                {
                    _ = context.Clients.Add(client.ToEntity());
                }

                _ = context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (IdentityResource resource in GetIdentityResources())
                {
                    _ = context.IdentityResources.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (ApiResource resource in GetApiResources())
                {
                    _ = context.ApiResources.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }
        }

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
                    _log.LogError("Unable to create admin role : {0}", result.ToString());
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

                result = userMgr.AddClaimsAsync(adminUser, new Claim[]{
                                new Claim(JwtClaimTypes.Name, "System Administrator"),
                                new Claim(JwtClaimTypes.GivenName, "System"),
                                new Claim(JwtClaimTypes.FamilyName, "administrator"),
                                new Claim(JwtClaimTypes.Email, "admin@localhost.net"),
                                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                new Claim(JwtClaimTypes.WebSite, "123 NoWhere"),
                                new Claim(JwtClaimTypes.Address, /*lang=json*/ @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                            }).Result;
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

        private IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        private IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "My API")
            };
        }

        // clients want to access resources (aka scopes)
        private IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
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
                new Client
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
                new Client
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
            };
        }
        #endregion Identity Server Configuration Object Creators

    }
}