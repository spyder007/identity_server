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
        [HttpGet]
        [ProducesResponseType<ApiResourceSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<ApiResourceSummaryDto>(dbContext.ApiResources.AsQueryable()));

        [HttpGet("{id:int}")]
        [ProducesResponseType<ApiResourceDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var entity = dbContext.ApiResources.FirstOrDefault(r => r.Id == id);
            return entity is null ? NotFound() : Ok(Mapper.Map<ApiResourceDto>(entity));
        }

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

        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveApiResourceDto dto)
        {
            var entity = dbContext.ApiResources.FirstOrDefault(r => r.Id == id);
            if (entity is null) return NotFound();
            _ = Mapper.Map(dto, entity);
            entity.Updated = DateTime.UtcNow;
            _ = dbContext.ApiResources.Update(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = dbContext.ApiResources.FirstOrDefault(r => r.Id == id);
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
        [HttpGet]
        [ProducesResponseType<ApiResourceClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourceClaimDto>(resource.UserClaims.AsQueryable()));
        }

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

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourceClaimDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourceClaimDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim>(dto);
            entity.ApiResourceId = apiResourceId;
            resource.UserClaims.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourceClaimDto>(entity));
        }

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
        [HttpGet]
        [ProducesResponseType<ApiResourcePropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourcePropertyDto>(resource.Properties.AsQueryable()));
        }

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

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourcePropertyDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourcePropertyDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceProperty>(dto);
            entity.ApiResourceId = apiResourceId;
            resource.Properties.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourcePropertyDto>(entity));
        }

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
        [HttpGet]
        [ProducesResponseType<ApiResourceScopeDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Scopes).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourceScopeDto>(resource.Scopes.AsQueryable()));
        }

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

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ApiResourceScopeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int apiResourceId, [FromBody] SaveApiResourceScopeDto dto)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Scopes).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiResourceScope>(dto);
            entity.ApiResourceId = apiResourceId;
            resource.Scopes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { apiResourceId, id = entity.Id }, Mapper.Map<ApiResourceScopeDto>(entity));
        }

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
        [HttpGet]
        [ProducesResponseType<ApiResourceSecretDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int apiResourceId)
        {
            var resource = await dbContext.ApiResources.Include(r => r.Secrets).FirstOrDefaultAsync(r => r.Id == apiResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<ApiResourceSecretDto>(resource.Secrets.AsQueryable()));
        }

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
