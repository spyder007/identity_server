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
        /// <summary>Gets a summary of all identity resources.</summary>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the collection of identity resource summaries.</returns>
        [HttpGet]
        [ProducesResponseType<IdentityResourceSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<IdentityResourceSummaryDto>(dbContext.IdentityResources.AsQueryable()));

        /// <summary>Gets the identity resource with the specified identifier.</summary>
        /// <param name="id">The identifier of the identity resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the identity resource, or <see cref="StatusCodes.Status404NotFound"/> if it does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<IdentityResourceDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var entity = dbContext.IdentityResources.FirstOrDefault(r => r.Id == id);
            return entity is null ? NotFound() : Ok(Mapper.Map<IdentityResourceDto>(entity));
        }

        /// <summary>Creates a new identity resource.</summary>
        /// <param name="dto">The identity resource details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created identity resource, or <see cref="StatusCodes.Status400BadRequest"/> if the request is invalid.</returns>
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

        /// <summary>Updates the specified identity resource.</summary>
        /// <param name="id">The identifier of the identity resource to update.</param>
        /// <param name="dto">The updated identity resource details.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource does not exist.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveIdentityResourceDto dto)
        {
            var entity = await dbContext.IdentityResources.FirstOrDefaultAsync(r => r.Id == id);
            if (entity is null) return NotFound();
            _ = Mapper.Map(dto, entity);
            entity.Updated = DateTime.UtcNow;
            _ = dbContext.IdentityResources.Update(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Deletes the specified identity resource.</summary>
        /// <param name="id">The identifier of the identity resource to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await dbContext.IdentityResources.FirstOrDefaultAsync(r => r.Id == id);
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
        /// <summary>Gets all user claims for the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the identity resource claims, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<IdentityResourceClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int identityResourceId)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.UserClaims).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<IdentityResourceClaimDto>(resource.UserClaims.AsQueryable()));
        }

        /// <summary>Gets the user claim with the specified identifier for the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <param name="id">The identifier of the identity resource claim.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the identity resource claim, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource or claim does not exist.</returns>
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

        /// <summary>Adds a new user claim to the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <param name="dto">The claim details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created claim, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource does not exist.</returns>
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

        /// <summary>Deletes the specified user claim from the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <param name="id">The identifier of the identity resource claim to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource or claim does not exist.</returns>
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
        /// <summary>Gets all custom properties for the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the identity resource properties, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<IdentityResourcePropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int identityResourceId)
        {
            var resource = await dbContext.IdentityResources.Include(r => r.Properties).FirstOrDefaultAsync(r => r.Id == identityResourceId);
            if (resource is null) return NotFound();
            return Ok(Mapper.ProjectTo<IdentityResourcePropertyDto>(resource.Properties.AsQueryable()));
        }

        /// <summary>Gets the custom property with the specified identifier for the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <param name="id">The identifier of the identity resource property.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the identity resource property, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource or property does not exist.</returns>
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

        /// <summary>Adds a new custom property to the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <param name="dto">The property details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created property, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource does not exist.</returns>
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

        /// <summary>Deletes the specified custom property from the specified identity resource.</summary>
        /// <param name="identityResourceId">The identifier of the parent identity resource.</param>
        /// <param name="id">The identifier of the identity resource property to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the identity resource or property does not exist.</returns>
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
