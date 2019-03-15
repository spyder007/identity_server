using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace spydersoft.Identity.Data
{
    public class DatabaseInitializer
    {
        private readonly IApplicationBuilder _app;

        public DatabaseInitializer(IApplicationBuilder app)
        {
            _app = app;
        }

        public void InitializeDatabase()
        {
            using (var serviceScope = _app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                PerformDatabaseMigrations(serviceScope);
                SeedDatabase(serviceScope);
            }
        }

        private void SeedDatabase(IServiceScope serviceScope)
        {
            ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                foreach (var client in GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }

        private void PerformDatabaseMigrations(IServiceScope serviceScope)
        {
            Task appDbTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>());

            Task persistedGrantTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>());

            Task configTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>());


            Task.WaitAll(appDbTask, persistedGrantTask, configTask);
        }

        private async Task DoMigrationIfNeeded(DbContext context)
        {
            bool hasMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
            if (hasMigrations)
            {
                await context.Database.MigrateAsync();
            }
        }
        // scopes define the resources in your system
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

        public void InitializeAutomapper()
        {
            Data.AutoMapper.Initialize();
        }
    }
}
