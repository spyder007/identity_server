using System;
using System.Linq;
using System.Threading.Tasks;

using Asp.Versioning;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Admin.Api.Models.ApiResources;

using ApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>REST API controller for managing API resources.</summary>
    [ApiVersion("1.0")]
    public class ApiResourcesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets a summary of all API resources.</summary>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the collection of API resource summaries.</returns>
        [HttpGet]
        [ProducesResponseType<ApiResourceSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<ApiResourceSummaryDto>(dbContext.ApiResources.AsQueryable()));

        /// <summary>Gets the API resource with the specified identifier.</summary>
        /// <param name="id">The identifier of the API resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource, or <see cref="StatusCodes.Status404NotFound"/> if it does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ApiResourceDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var entity = dbContext.ApiResources.FirstOrDefault(r => r.Id == id);
            return entity is null ? NotFound() : Ok(Mapper.Map<ApiResourceDto>(entity));
        }

        /// <summary>Gets the API resource with the specified natural <c>Name</c> key.</summary>
        /// <param name="name">The name of the API resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource, or <see cref="StatusCodes.Status404NotFound"/> if it does not exist.</returns>
        [HttpGet("by-name/{name}")]
        [ProducesResponseType<ApiResourceDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByName(string name)
        {
            var entity = dbContext.ApiResources.FirstOrDefault(r => r.Name == name);
            return entity is null ? NotFound() : Ok(Mapper.Map<ApiResourceDto>(entity));
        }

        /// <summary>Creates a new API resource.</summary>
        /// <param name="dto">The API resource details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created API resource, or <see cref="StatusCodes.Status400BadRequest"/> if the request is invalid.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourceDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SaveApiResourceDto dto)
        {
            var entity = Mapper.Map<ApiResource>(dto);
            entity.Created = DateTime.UtcNow;
            _ = dbContext.ApiResources.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Mapper.Map<ApiResourceDto>(entity));
        }

        /// <summary>Updates the specified API resource.</summary>
        /// <param name="id">The identifier of the API resource to update.</param>
        /// <param name="dto">The updated API resource details.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveApiResourceDto dto)
        {
            var entity = await dbContext.ApiResources.FirstOrDefaultAsync(r => r.Id == id);
            if (entity is null) return NotFound();
            _ = Mapper.Map(dto, entity);
            entity.Updated = DateTime.UtcNow;
            _ = dbContext.ApiResources.Update(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Deletes the specified API resource.</summary>
        /// <param name="id">The identifier of the API resource to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await dbContext.ApiResources.FirstOrDefaultAsync(r => r.Id == id);
            if (entity is null) return NotFound();
            _ = dbContext.ApiResources.Remove(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Claims
    // ---------------------------------------------------------------------------

    /// <summary>Manages user claims associated with an API resource.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/apiresources/{apiResourceId:int}/claims")]
    public class ApiResourceClaimsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets all user claims for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource claims, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ApiResourceClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourceClaimDto>(resource.UserClaims.AsQueryable()));
        }

        /// <summary>Gets the user claim with the specified identifier for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource claim.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource claim, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or claim does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ApiResourceClaimDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.UserClaims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ApiResourceClaimDto>(item));
        }

        /// <summary>Adds a new user claim to the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="dto">The claim details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created claim, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourceClaimDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourceClaimDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            if (resource.UserClaims.Any(x => x.Type == dto.Type))
                return Conflict($"A claim with type '{dto.Type}' already exists for this API resource.");
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim>(dto);
            entity.ApiResourceId = apiResourceId;
            resource.UserClaims.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourceClaimDto>(entity));
        }

        /// <summary>Deletes the specified user claim from the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource claim to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or claim does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.UserClaims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            resource!.UserClaims.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Properties
    // ---------------------------------------------------------------------------

    /// <summary>Manages custom properties of an API resource.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/apiresources/{apiResourceId:int}/properties")]
    public class ApiResourcePropertiesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets all custom properties for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource properties, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ApiResourcePropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourcePropertyDto>(resource.Properties.AsQueryable()));
        }

        /// <summary>Gets the custom property with the specified identifier for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource property.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource property, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or property does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ApiResourcePropertyDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ApiResourcePropertyDto>(item));
        }

        /// <summary>Adds a new custom property to the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="dto">The property details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created property, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourcePropertyDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourcePropertyDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            if (resource.Properties.Any(x => x.Key == dto.Key))
                return Conflict($"A property with key '{dto.Key}' already exists for this API resource.");
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceProperty>(dto);
            entity.ApiResourceId = apiResourceId;
            resource.Properties.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourcePropertyDto>(entity));
        }

        /// <summary>Deletes the specified custom property from the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource property to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or property does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            resource!.Properties.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Scopes
    // ---------------------------------------------------------------------------

    /// <summary>Manages scopes associated with an API resource.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/apiresources/{apiResourceId:int}/scopes")]
    public class ApiResourceScopesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets all scopes for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource scopes, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ApiResourceScopeDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Scopes).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourceScopeDto>(resource.Scopes.AsQueryable()));
        }

        /// <summary>Gets the scope with the specified identifier for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource scope.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource scope, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or scope does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ApiResourceScopeDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Scopes).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.Scopes.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ApiResourceScopeDto>(item));
        }

        /// <summary>Adds a new scope to the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="dto">The scope details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created scope, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourceScopeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourceScopeDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Scopes).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            if (resource.Scopes.Any(x => x.Scope == dto.Scope))
                return Conflict($"A scope '{dto.Scope}' already exists for this API resource.");
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceScope>(dto);
            entity.ApiResourceId = apiResourceId;
            resource.Scopes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourceScopeDto>(entity));
        }

        /// <summary>Deletes the specified scope from the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource scope to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or scope does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Scopes).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.Scopes.Find(x => x.Id == id);
            if (item is null) return NotFound();
            resource!.Scopes.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Secrets
    // ---------------------------------------------------------------------------

    /// <summary>Manages secrets for an API resource.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/apiresources/{apiResourceId:int}/secrets")]
    public class ApiResourceSecretsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets all secrets for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource secrets, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ApiResourceSecretDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Secrets).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourceSecretDto>(resource.Secrets.AsQueryable()));
        }

        /// <summary>Gets the secret with the specified identifier for the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource secret.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the API resource secret, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or secret does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ApiResourceSecretDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Secrets).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.Secrets.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ApiResourceSecretDto>(item));
        }

        /// <summary>Adds a new secret to the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="dto">The secret details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created secret, or <see cref="StatusCodes.Status404NotFound"/> if the API resource does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourceSecretDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourceSecretDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Secrets).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceSecret>(dto);
            entity.ApiResourceId = apiResourceId;
            entity.Created = DateTime.UtcNow;
            resource.Secrets.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourceSecretDto>(entity));
        }

        /// <summary>Deletes the specified secret from the specified API resource.</summary>
        /// <param name="apiResourceId">The identifier of the parent API resource.</param>
        /// <param name="id">The identifier of the API resource secret to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the API resource or secret does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int apiResourceId, int id)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Secrets).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            var item = resource?.Secrets.Find(x => x.Id == id);
            if (item is null) return NotFound();
            resource!.Secrets.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
