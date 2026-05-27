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

using Spydersoft.Identity.Admin.Api.Models.IdentityResources;

using IdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>REST API controller for managing identity resources.</summary>
    [ApiVersion("1.0")]
    public class IdentityResourcesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<IdentityResourceSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<IdentityResourceSummaryDto>(dbContext.IdentityResources.AsQueryable()));

        [HttpGet("{id:int}")]
        [ProducesResponseType<IdentityResourceDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var entity = dbContext.IdentityResources.FirstOrDefault(r => r.Id == id);
            return entity is null ? NotFound() : Ok(Mapper.Map<IdentityResourceDto>(entity));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<IdentityResourceDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SaveIdentityResourceDto dto)
        {
            var entity = Mapper.Map<IdentityResource>(dto);
            entity.Created = DateTime.UtcNow;
            _ = dbContext.IdentityResources.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Mapper.Map<IdentityResourceDto>(entity));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveIdentityResourceDto dto)
        {
            var entity = dbContext.IdentityResources.FirstOrDefault(r => r.Id == id);
            if (entity is null) return NotFound();
            _ = Mapper.Map(dto, entity);
            entity.Updated = DateTime.UtcNow;
            _ = dbContext.IdentityResources.Update(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = dbContext.IdentityResources.FirstOrDefault(r => r.Id == id);
            if (entity is null) return NotFound();
            _ = dbContext.IdentityResources.Remove(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Claims
    // ---------------------------------------------------------------------------

    /// <summary>Manages user claims associated with an identity resource.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/identityresources/{identityResourceId:int}/claims")]
    public class IdentityResourceClaimsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<IdentityResourceClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int identityResourceId)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<IdentityResourceClaimDto>(resource.UserClaims.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<IdentityResourceClaimDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int identityResourceId, int id)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            var item = resource?.UserClaims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<IdentityResourceClaimDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<IdentityResourceClaimDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int identityResourceId, [FromBody] SaveIdentityResourceClaimDto dto)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            if (resource is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.IdentityResourceClaim>(dto);
            entity.IdentityResourceId = identityResourceId;
            resource.UserClaims.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { identityResourceId, id = entity.Id }, Mapper.Map<IdentityResourceClaimDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int identityResourceId, int id)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == identityResourceId);
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

    /// <summary>Manages custom properties of an identity resource.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/identityresources/{identityResourceId:int}/properties")]
    public class IdentityResourcePropertiesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<IdentityResourcePropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int identityResourceId)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<IdentityResourcePropertyDto>(resource.Properties.AsQueryable()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<IdentityResourcePropertyDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int identityResourceId, int id)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            var item = resource?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<IdentityResourcePropertyDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<IdentityResourcePropertyDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int identityResourceId, [FromBody] SaveIdentityResourcePropertyDto dto)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            if (resource is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.IdentityResourceProperty>(dto);
            entity.IdentityResourceId = identityResourceId;
            resource.Properties.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { identityResourceId, id = entity.Id }, Mapper.Map<IdentityResourcePropertyDto>(entity));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int identityResourceId, int id)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            var item = resource?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            resource!.Properties.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
