using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Options;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Spydersoft.Identity.Admin.Api.Controllers.V1;
using Spydersoft.Identity.Admin.Api.Data;
using Spydersoft.Identity.Admin.Api.Models.ApiResources;
using Spydersoft.Identity.Admin.Api.Models.Clients;
using Spydersoft.Identity.Admin.Api.Models.IdentityResources;
using Spydersoft.Identity.Admin.Api.Models.Scopes;

namespace Spydersoft.Identity.Tests
{
    /// <summary>
    /// Covers the natural-key lookups and duplicate-prevention added to the admin API
    /// so a Terraform provider can rely on import and idempotent apply.
    /// </summary>
    public class AdminApiTerraformSupportTests
    {
        private ConfigurationDbContext _db = null!;
        private IMapper _mapper = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new ConfigurationDbContext(options) { StoreOptions = new ConfigurationStoreOptions() };
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiAutoMapperProfile>(), NullLoggerFactory.Instance).CreateMapper();
        }

        [TearDown]
        public void TearDown() => _db.Dispose();

        // ---- Natural-key lookups ----

        [Test]
        public async Task ClientGetByClientId_ReturnsClientOrNotFound()
        {
            _db.Clients.Add(new Client { ClientId = "my-client", ClientName = "My Client" });
            await _db.SaveChangesAsync();
            var controller = new ClientsController(_db, _mapper);

            Assert.Multiple(() =>
            {
                Assert.That(controller.GetByClientId("my-client"), Is.InstanceOf<OkObjectResult>());
                Assert.That(controller.GetByClientId("missing"), Is.InstanceOf<NotFoundResult>());
            });
        }

        [Test]
        public async Task ApiResourceGetByName_ReturnsResourceOrNotFound()
        {
            _db.ApiResources.Add(new ApiResource { Name = "my-api" });
            await _db.SaveChangesAsync();
            var controller = new ApiResourcesController(_db, _mapper);

            Assert.Multiple(() =>
            {
                Assert.That(controller.GetByName("my-api"), Is.InstanceOf<OkObjectResult>());
                Assert.That(controller.GetByName("missing"), Is.InstanceOf<NotFoundResult>());
            });
        }

        [Test]
        public async Task IdentityResourceGetByName_ReturnsResourceOrNotFound()
        {
            _db.IdentityResources.Add(new IdentityResource { Name = "my-identity" });
            await _db.SaveChangesAsync();
            var controller = new IdentityResourcesController(_db, _mapper);

            Assert.Multiple(() =>
            {
                Assert.That(controller.GetByName("my-identity"), Is.InstanceOf<OkObjectResult>());
                Assert.That(controller.GetByName("missing"), Is.InstanceOf<NotFoundResult>());
            });
        }

        [Test]
        public async Task ScopeGetByName_ReturnsScopeOrNotFound()
        {
            _db.ApiScopes.Add(new ApiScope { Name = "my-scope" });
            await _db.SaveChangesAsync();
            var controller = new ScopesController(_db, _mapper);

            Assert.Multiple(() =>
            {
                Assert.That(controller.GetByName("my-scope"), Is.InstanceOf<OkObjectResult>());
                Assert.That(controller.GetByName("missing"), Is.InstanceOf<NotFoundResult>());
            });
        }

        // ---- Client sub-resource duplicates ----

        [Test]
        public async Task ClientClaims_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.Claims = [new ClientClaim { Type = "t", Value = "v" }]);
            var controller = new ClientClaimsController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientClaimDto { Type = "t", Value = "v" }),
                () => controller.Create(client.Id, new SaveClientClaimDto { Type = "t", Value = "other" }));
        }

        [Test]
        public async Task ClientCorsOrigins_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.AllowedCorsOrigins = [new ClientCorsOrigin { Origin = "https://a" }]);
            var controller = new ClientCorsOriginsController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientCorsOriginDto { Origin = "https://a" }),
                () => controller.Create(client.Id, new SaveClientCorsOriginDto { Origin = "https://b" }));
        }

        [Test]
        public async Task ClientGrantTypes_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.AllowedGrantTypes = [new ClientGrantType { GrantType = "client_credentials" }]);
            var controller = new ClientGrantTypesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientGrantTypeDto { GrantType = "client_credentials" }),
                () => controller.Create(client.Id, new SaveClientGrantTypeDto { GrantType = "code" }));
        }

        [Test]
        public async Task ClientIdpRestrictions_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.IdentityProviderRestrictions = [new ClientIdPRestriction { Provider = "google" }]);
            var controller = new ClientIdpRestrictionsController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientIdpRestrictionDto { Provider = "google" }),
                () => controller.Create(client.Id, new SaveClientIdpRestrictionDto { Provider = "github" }));
        }

        [Test]
        public async Task ClientPostLogoutRedirectUris_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.PostLogoutRedirectUris = [new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "https://a/out" }]);
            var controller = new ClientPostLogoutRedirectUrisController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientPostLogoutRedirectUriDto { PostLogoutRedirectUri = "https://a/out" }),
                () => controller.Create(client.Id, new SaveClientPostLogoutRedirectUriDto { PostLogoutRedirectUri = "https://b/out" }));
        }

        [Test]
        public async Task ClientProperties_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.Properties = [new ClientProperty { Key = "k", Value = "v" }]);
            var controller = new ClientPropertiesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientPropertyDto { Key = "k", Value = "v2" }),
                () => controller.Create(client.Id, new SaveClientPropertyDto { Key = "k2", Value = "v" }));
        }

        [Test]
        public async Task ClientRedirectUris_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.RedirectUris = [new ClientRedirectUri { RedirectUri = "https://a/cb" }]);
            var controller = new ClientRedirectUrisController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientRedirectUriDto { RedirectUri = "https://a/cb" }),
                () => controller.Create(client.Id, new SaveClientRedirectUriDto { RedirectUri = "https://b/cb" }));
        }

        [Test]
        public async Task ClientScopes_DuplicateConflicts_NewSucceeds()
        {
            var client = await SeedClient(c => c.AllowedScopes = [new ClientScope { Scope = "openid" }]);
            var controller = new ClientScopesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(client.Id, new SaveClientScopeDto { Scope = "openid" }),
                () => controller.Create(client.Id, new SaveClientScopeDto { Scope = "profile" }));
        }

        // ---- API resource sub-resource duplicates ----

        [Test]
        public async Task ApiResourceClaims_DuplicateConflicts_NewSucceeds()
        {
            var resource = await SeedApiResource(r => r.UserClaims = [new ApiResourceClaim { Type = "t" }]);
            var controller = new ApiResourceClaimsController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(resource.Id, new SaveApiResourceClaimDto { Type = "t" }),
                () => controller.Create(resource.Id, new SaveApiResourceClaimDto { Type = "t2" }));
        }

        [Test]
        public async Task ApiResourceProperties_DuplicateConflicts_NewSucceeds()
        {
            var resource = await SeedApiResource(r => r.Properties = [new ApiResourceProperty { Key = "k", Value = "v" }]);
            var controller = new ApiResourcePropertiesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(resource.Id, new SaveApiResourcePropertyDto { Key = "k", Value = "v" }),
                () => controller.Create(resource.Id, new SaveApiResourcePropertyDto { Key = "k2", Value = "v" }));
        }

        [Test]
        public async Task ApiResourceScopes_DuplicateConflicts_NewSucceeds()
        {
            var resource = await SeedApiResource(r => r.Scopes = [new ApiResourceScope { Scope = "s" }]);
            var controller = new ApiResourceScopesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(resource.Id, new SaveApiResourceScopeDto { Scope = "s" }),
                () => controller.Create(resource.Id, new SaveApiResourceScopeDto { Scope = "s2" }));
        }

        // ---- Identity resource sub-resource duplicates ----

        [Test]
        public async Task IdentityResourceClaims_DuplicateConflicts_NewSucceeds()
        {
            var resource = await SeedIdentityResource(r => r.UserClaims = [new IdentityResourceClaim { Type = "t" }]);
            var controller = new IdentityResourceClaimsController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(resource.Id, new SaveIdentityResourceClaimDto { Type = "t" }),
                () => controller.Create(resource.Id, new SaveIdentityResourceClaimDto { Type = "t2" }));
        }

        [Test]
        public async Task IdentityResourceProperties_DuplicateConflicts_NewSucceeds()
        {
            var resource = await SeedIdentityResource(r => r.Properties = [new IdentityResourceProperty { Key = "k", Value = "v" }]);
            var controller = new IdentityResourcePropertiesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(resource.Id, new SaveIdentityResourcePropertyDto { Key = "k", Value = "v" }),
                () => controller.Create(resource.Id, new SaveIdentityResourcePropertyDto { Key = "k2", Value = "v" }));
        }

        // ---- Scope sub-resource duplicates ----

        [Test]
        public async Task ScopeClaims_DuplicateConflicts_NewSucceeds()
        {
            var scope = await SeedScope(s => s.UserClaims = [new ApiScopeClaim { Type = "t" }]);
            var controller = new ScopeClaimsController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(scope.Id, new SaveScopeClaimDto { Type = "t" }),
                () => controller.Create(scope.Id, new SaveScopeClaimDto { Type = "t2" }));
        }

        [Test]
        public async Task ScopeProperties_DuplicateConflicts_NewSucceeds()
        {
            var scope = await SeedScope(s => s.Properties = [new ApiScopeProperty { Key = "k", Value = "v" }]);
            var controller = new ScopePropertiesController(_db, _mapper);

            await AssertDuplicateThenNew(
                () => controller.Create(scope.Id, new SaveScopePropertyDto { Key = "k", Value = "v" }),
                () => controller.Create(scope.Id, new SaveScopePropertyDto { Key = "k2", Value = "v" }));
        }

        // ---- Seeder ----

        [Test]
        public void SeederIncludesTerraformProviderClient()
        {
            var client = DataSeeder.Seeding.Clients.All("frontend-secret")
                .Single(c => c.ClientId == "terraform-provider");

            Assert.Multiple(() =>
            {
                Assert.That(client.AllowedGrantTypes, Does.Contain("client_credentials"));
                Assert.That(client.AllowedScopes, Is.EquivalentTo(new[] { "identity:admin:read", "identity:admin:write" }));
                Assert.That(client.ClientSecrets, Has.Count.EqualTo(1));
            });
        }

        // ---- Helpers ----

        private static async Task AssertDuplicateThenNew(Func<Task<IActionResult>> duplicate, Func<Task<IActionResult>> fresh)
        {
            Assert.That(await duplicate(), Is.InstanceOf<ConflictObjectResult>());
            Assert.That(await fresh(), Is.Not.InstanceOf<ConflictObjectResult>().And.Not.InstanceOf<NotFoundResult>());
        }

        private async Task<Client> SeedClient(Action<Client> configure)
        {
            var client = new Client { ClientId = Guid.NewGuid().ToString(), ClientName = "c" };
            configure(client);
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return client;
        }

        private async Task<ApiResource> SeedApiResource(Action<ApiResource> configure)
        {
            var resource = new ApiResource { Name = Guid.NewGuid().ToString() };
            configure(resource);
            _db.ApiResources.Add(resource);
            await _db.SaveChangesAsync();
            return resource;
        }

        private async Task<IdentityResource> SeedIdentityResource(Action<IdentityResource> configure)
        {
            var resource = new IdentityResource { Name = Guid.NewGuid().ToString() };
            configure(resource);
            _db.IdentityResources.Add(resource);
            await _db.SaveChangesAsync();
            return resource;
        }

        private async Task<ApiScope> SeedScope(Action<ApiScope> configure)
        {
            var scope = new ApiScope { Name = Guid.NewGuid().ToString() };
            configure(scope);
            _db.ApiScopes.Add(scope);
            await _db.SaveChangesAsync();
            return scope;
        }
    }
}
