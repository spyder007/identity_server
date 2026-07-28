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
        /// <summary>Gets all claims for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's claims, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientClaimDto>(client.Claims.AsQueryable()));
        }

        /// <summary>Gets a single claim for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the claim.</param>
        /// <returns>The requested claim, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new claim for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The claim to create.</param>
        /// <returns>The created claim, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientClaimDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientClaimDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.Claims.Any(x => x.Type == dto.Type && x.Value == dto.Value))
                return Conflict($"A claim with type '{dto.Type}' and value '{dto.Value}' already exists for this client.");
            var entity = Mapper.Map<ClientClaim>(dto);
            entity.ClientId = clientId;
            client.Claims.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientClaimDto>(entity));
        }

        /// <summary>Deletes a claim from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the claim to delete.</param>
        /// <returns>No content on success, or 404 if the claim does not exist.</returns>
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
        /// <summary>Gets all allowed CORS origins for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's CORS origins, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientCorsOriginDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedCorsOrigins).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientCorsOriginDto>(client.AllowedCorsOrigins.AsQueryable()));
        }

        /// <summary>Gets a single allowed CORS origin for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the CORS origin.</param>
        /// <returns>The requested CORS origin, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new allowed CORS origin for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The CORS origin to create.</param>
        /// <returns>The created CORS origin, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientCorsOriginDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientCorsOriginDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedCorsOrigins).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.AllowedCorsOrigins.Any(x => x.Origin == dto.Origin))
                return Conflict($"A CORS origin '{dto.Origin}' already exists for this client.");
            var entity = Mapper.Map<ClientCorsOrigin>(dto);
            entity.ClientId = clientId;
            client.AllowedCorsOrigins.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientCorsOriginDto>(entity));
        }

        /// <summary>Deletes an allowed CORS origin from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the CORS origin to delete.</param>
        /// <returns>No content on success, or 404 if the CORS origin does not exist.</returns>
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
        /// <summary>Gets all allowed grant types for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's grant types, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientGrantTypeDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedGrantTypes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientGrantTypeDto>(client.AllowedGrantTypes.AsQueryable()));
        }

        /// <summary>Gets a single allowed grant type for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the grant type.</param>
        /// <returns>The requested grant type, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new allowed grant type for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The grant type to create.</param>
        /// <returns>The created grant type, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientGrantTypeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientGrantTypeDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedGrantTypes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.AllowedGrantTypes.Any(x => x.GrantType == dto.GrantType))
                return Conflict($"A grant type '{dto.GrantType}' already exists for this client.");
            var entity = Mapper.Map<ClientGrantType>(dto);
            entity.ClientId = clientId;
            client.AllowedGrantTypes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientGrantTypeDto>(entity));
        }

        /// <summary>Deletes an allowed grant type from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the grant type to delete.</param>
        /// <returns>No content on success, or 404 if the grant type does not exist.</returns>
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
        /// <summary>Gets all identity provider restrictions for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's identity provider restrictions, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientIdpRestrictionDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.IdentityProviderRestrictions).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientIdpRestrictionDto>(client.IdentityProviderRestrictions.AsQueryable()));
        }

        /// <summary>Gets a single identity provider restriction for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the identity provider restriction.</param>
        /// <returns>The requested identity provider restriction, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new identity provider restriction for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The identity provider restriction to create.</param>
        /// <returns>The created identity provider restriction, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientIdpRestrictionDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientIdpRestrictionDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.IdentityProviderRestrictions).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.IdentityProviderRestrictions.Any(x => x.Provider == dto.Provider))
                return Conflict($"An identity provider restriction '{dto.Provider}' already exists for this client.");
            var entity = Mapper.Map<ClientIdPRestriction>(dto);
            entity.ClientId = clientId;
            client.IdentityProviderRestrictions.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientIdpRestrictionDto>(entity));
        }

        /// <summary>Deletes an identity provider restriction from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the identity provider restriction to delete.</param>
        /// <returns>No content on success, or 404 if the identity provider restriction does not exist.</returns>
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
        /// <summary>Gets all post-logout redirect URIs for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's post-logout redirect URIs, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientPostLogoutRedirectUriDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.PostLogoutRedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientPostLogoutRedirectUriDto>(client.PostLogoutRedirectUris.AsQueryable()));
        }

        /// <summary>Gets a single post-logout redirect URI for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the post-logout redirect URI.</param>
        /// <returns>The requested post-logout redirect URI, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new post-logout redirect URI for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The post-logout redirect URI to create.</param>
        /// <returns>The created post-logout redirect URI, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientPostLogoutRedirectUriDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientPostLogoutRedirectUriDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.PostLogoutRedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.PostLogoutRedirectUris.Any(x => x.PostLogoutRedirectUri == dto.PostLogoutRedirectUri))
                return Conflict($"A post-logout redirect URI '{dto.PostLogoutRedirectUri}' already exists for this client.");
            var entity = Mapper.Map<ClientPostLogoutRedirectUri>(dto);
            entity.ClientId = clientId;
            client.PostLogoutRedirectUris.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientPostLogoutRedirectUriDto>(entity));
        }

        /// <summary>Deletes a post-logout redirect URI from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the post-logout redirect URI to delete.</param>
        /// <returns>No content on success, or 404 if the post-logout redirect URI does not exist.</returns>
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
        /// <summary>Gets all custom properties for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's custom properties, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientPropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientPropertyDto>(client.Properties.AsQueryable()));
        }

        /// <summary>Gets a single custom property for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the property.</param>
        /// <returns>The requested property, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new custom property for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The property to create.</param>
        /// <returns>The created property, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientPropertyDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientPropertyDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.Properties.Any(x => x.Key == dto.Key))
                return Conflict($"A property with key '{dto.Key}' already exists for this client.");
            var entity = Mapper.Map<ClientProperty>(dto);
            entity.ClientId = clientId;
            client.Properties.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientPropertyDto>(entity));
        }

        /// <summary>Deletes a custom property from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the property to delete.</param>
        /// <returns>No content on success, or 404 if the property does not exist.</returns>
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
        /// <summary>Gets all redirect URIs for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's redirect URIs, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientRedirectUriDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.RedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientRedirectUriDto>(client.RedirectUris.AsQueryable()));
        }

        /// <summary>Gets a single redirect URI for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the redirect URI.</param>
        /// <returns>The requested redirect URI, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new redirect URI for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The redirect URI to create.</param>
        /// <returns>The created redirect URI, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientRedirectUriDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientRedirectUriDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.RedirectUris).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.RedirectUris.Any(x => x.RedirectUri == dto.RedirectUri))
                return Conflict($"A redirect URI '{dto.RedirectUri}' already exists for this client.");
            var entity = Mapper.Map<ClientRedirectUri>(dto);
            entity.ClientId = clientId;
            client.RedirectUris.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientRedirectUriDto>(entity));
        }

        /// <summary>Deletes a redirect URI from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the redirect URI to delete.</param>
        /// <returns>No content on success, or 404 if the redirect URI does not exist.</returns>
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
        /// <summary>Gets all allowed scopes for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's allowed scopes, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientScopeDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientScopeDto>(client.AllowedScopes.AsQueryable()));
        }

        /// <summary>Gets a single allowed scope for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the scope.</param>
        /// <returns>The requested scope, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new allowed scope for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The scope to create.</param>
        /// <returns>The created scope, or 404 if the client does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientScopeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int clientId, [FromBody] SaveClientScopeDto dto)
        {
            var client = await dbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            if (client.AllowedScopes.Any(x => x.Scope == dto.Scope))
                return Conflict($"A scope '{dto.Scope}' already exists for this client.");
            var entity = Mapper.Map<ClientScope>(dto);
            entity.ClientId = clientId;
            client.AllowedScopes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { clientId, id = entity.Id }, Mapper.Map<ClientScopeDto>(entity));
        }

        /// <summary>Deletes an allowed scope from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the scope to delete.</param>
        /// <returns>No content on success, or 404 if the scope does not exist.</returns>
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
        /// <summary>Gets all secrets for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>The client's secrets, or 404 if the client does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ClientSecretDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int clientId)
        {
            var client = await dbContext.Clients.Include(c => c.ClientSecrets).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) return NotFound();
            return Ok(Mapper.ProjectTo<ClientSecretDto>(client.ClientSecrets.AsQueryable()));
        }

        /// <summary>Gets a single secret for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the secret.</param>
        /// <returns>The requested secret, or 404 if it does not exist.</returns>
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

        /// <summary>Creates a new secret for the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="dto">The secret to create.</param>
        /// <returns>The created secret, or 404 if the client does not exist.</returns>
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

        /// <summary>Deletes a secret from the specified client.</summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="id">The identifier of the secret to delete.</param>
        /// <returns>No content on success, or 404 if the secret does not exist.</returns>
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
