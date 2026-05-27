using System.Linq;
using System.Threading.Tasks;

using Asp.Versioning;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Admin.Api.Models.Clients;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    // ---------------------------------------------------------------------------
    // Claims
    // ---------------------------------------------------------------------------

    /// <summary>Manages claims attached to a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/claims")]
    public class ClientClaimsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientClaimDto>(client.Claims.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientClaimDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.Claims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientClaimDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientClaimDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientClaimDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientClaim>(dto);
            entity.ClientId = clientId;
            client.Claims.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientClaimDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.Claims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.Claims.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // CORS Origins
    // ---------------------------------------------------------------------------

    /// <summary>Manages allowed CORS origins for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/corsorigins")]
    public class ClientCorsOriginsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientCorsOriginDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedCorsOrigins).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientCorsOriginDto>(client.AllowedCorsOrigins.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientCorsOriginDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedCorsOrigins).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.AllowedCorsOrigins.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientCorsOriginDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientCorsOriginDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientCorsOriginDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedCorsOrigins).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientCorsOrigin>(dto);
            entity.ClientId = clientId;
            client.AllowedCorsOrigins.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientCorsOriginDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedCorsOrigins).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.AllowedCorsOrigins.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.AllowedCorsOrigins.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Grant Types
    // ---------------------------------------------------------------------------

    /// <summary>Manages allowed grant types for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/granttypes")]
    public class ClientGrantTypesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientGrantTypeDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedGrantTypes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientGrantTypeDto>(client.AllowedGrantTypes.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientGrantTypeDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedGrantTypes).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.AllowedGrantTypes.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientGrantTypeDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientGrantTypeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientGrantTypeDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedGrantTypes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientGrantType>(dto);
            entity.ClientId = clientId;
            client.AllowedGrantTypes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientGrantTypeDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedGrantTypes).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.AllowedGrantTypes.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.AllowedGrantTypes.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // IDP Restrictions
    // ---------------------------------------------------------------------------

    /// <summary>Manages identity provider restrictions for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/idprestrictions")]
    public class ClientIdpRestrictionsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientIdpRestrictionDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.IdentityProviderRestrictions).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientIdpRestrictionDto>(client.IdentityProviderRestrictions.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientIdpRestrictionDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.IdentityProviderRestrictions).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.IdentityProviderRestrictions.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientIdpRestrictionDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientIdpRestrictionDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientIdpRestrictionDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.IdentityProviderRestrictions).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientIdPRestriction>(dto);
            entity.ClientId = clientId;
            client.IdentityProviderRestrictions.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientIdpRestrictionDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.IdentityProviderRestrictions).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.IdentityProviderRestrictions.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.IdentityProviderRestrictions.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Post Logout Redirect URIs
    // ---------------------------------------------------------------------------

    /// <summary>Manages post-logout redirect URIs for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/postlogoutredirecturis")]
    public class ClientPostLogoutRedirectUrisController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientPostLogoutRedirectUriDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.PostLogoutRedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientPostLogoutRedirectUriDto>(client.PostLogoutRedirectUris.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientPostLogoutRedirectUriDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.PostLogoutRedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.PostLogoutRedirectUris.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientPostLogoutRedirectUriDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientPostLogoutRedirectUriDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientPostLogoutRedirectUriDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.PostLogoutRedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientPostLogoutRedirectUri>(dto);
            entity.ClientId = clientId;
            client.PostLogoutRedirectUris.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientPostLogoutRedirectUriDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.PostLogoutRedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.PostLogoutRedirectUris.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.PostLogoutRedirectUris.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Properties
    // ---------------------------------------------------------------------------

    /// <summary>Manages custom properties for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/properties")]
    public class ClientPropertiesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientPropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientPropertyDto>(client.Properties.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientPropertyDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientPropertyDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientPropertyDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientPropertyDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientProperty>(dto);
            entity.ClientId = clientId;
            client.Properties.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientPropertyDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.Properties.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Redirect URIs
    // ---------------------------------------------------------------------------

    /// <summary>Manages redirect URIs for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/redirecturis")]
    public class ClientRedirectUrisController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientRedirectUriDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.RedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientRedirectUriDto>(client.RedirectUris.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientRedirectUriDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.RedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.RedirectUris.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientRedirectUriDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientRedirectUriDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientRedirectUriDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.RedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientRedirectUri>(dto);
            entity.ClientId = clientId;
            client.RedirectUris.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientRedirectUriDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.RedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.RedirectUris.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.RedirectUris.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Scopes
    // ---------------------------------------------------------------------------

    /// <summary>Manages allowed scopes for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/scopes")]
    public class ClientScopesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientScopeDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientScopeDto>(client.AllowedScopes.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientScopeDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.AllowedScopes.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientScopeDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientScopeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientScopeDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientScope>(dto);
            entity.ClientId = clientId;
            client.AllowedScopes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientScopeDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.AllowedScopes.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.AllowedScopes.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Secrets
    // ---------------------------------------------------------------------------

    /// <summary>Manages secrets for a client.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clients/{clientId:int}/secrets")]
    public class ClientSecretsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<ClientSecretDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.ClientSecrets).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientSecretDto>(client.ClientSecrets.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientSecretDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.ClientSecrets).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.ClientSecrets.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ClientSecretDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientSecretDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientSecretDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.ClientSecrets).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            var entity = Mapper.Map<ClientSecret>(dto);
            entity.ClientId = clientId;
            entity.Created = System.DateTime.UtcNow;
            client.ClientSecrets.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientSecretDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int clientId, int id)
        {
            var client = await dbContext.Clients.Include(c => c.ClientSecrets).FirstOrDefaultAsync(c => c.Id == clientId);
            var item = client?.ClientSecrets.Find(x => x.Id == id);
            if (item is null) return NotFound();
            client!.ClientSecrets.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
