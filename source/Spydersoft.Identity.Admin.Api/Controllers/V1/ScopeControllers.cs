using System.Linq;
using System.Threading.Tasks;

using Asp.Versioning;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Admin.Api.Models.Scopes;

using ApiScope = Duende.IdentityServer.EntityFramework.Entities.ApiScope;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>REST API controller for managing API scopes.</summary>
    [ApiVersion("1.0")]
    public class ScopesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets a summary of all API scopes.</summary>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the collection of scope summaries.</returns>
        [HttpGet]
        [ProducesResponseType<ScopeSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<ScopeSummaryDto>(dbContext.ApiScopes.AsQueryable()));

        /// <summary>Gets the API scope with the specified identifier.</summary>
        /// <param name="id">The identifier of the scope.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the scope, or <see cref="StatusCodes.Status404NotFound"/> if it does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ScopeDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var entity = dbContext.ApiScopes.FirstOrDefault(s => s.Id == id);
            return entity is null ? NotFound() : Ok(Mapper.Map<ScopeDto>(entity));
        }

        /// <summary>Creates a new API scope.</summary>
        /// <param name="dto">The scope details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created scope, or <see cref="StatusCodes.Status400BadRequest"/> if the request is invalid.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ScopeDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SaveScopeDto dto)
        {
            var entity = Mapper.Map<ApiScope>(dto);
            _ = dbContext.ApiScopes.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Mapper.Map<ScopeDto>(entity));
        }

        /// <summary>Updates the specified API scope.</summary>
        /// <param name="id">The identifier of the scope to update.</param>
        /// <param name="dto">The updated scope details.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the scope does not exist.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveScopeDto dto)
        {
            var entity = await dbContext.ApiScopes.FirstOrDefaultAsync(s => s.Id == id);
            if (entity is null) return NotFound();
            _ = Mapper.Map(dto, entity);
            _ = dbContext.ApiScopes.Update(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Deletes the specified API scope.</summary>
        /// <param name="id">The identifier of the scope to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the scope does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await dbContext.ApiScopes.FirstOrDefaultAsync(s => s.Id == id);
            if (entity is null) return NotFound();
            _ = dbContext.ApiScopes.Remove(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Claims
    // ---------------------------------------------------------------------------

    /// <summary>Manages user claims associated with a scope.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/scopes/{scopeId:int}/claims")]
    public class ScopeClaimsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets all user claims for the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the scope claims, or <see cref="StatusCodes.Status404NotFound"/> if the scope does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ScopeClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int scopeId)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.UserClaims).FirstOrDefaultAsync(s => s.Id == scopeId);
            if (scope is null) return NotFound();
            return Ok(Mapper.ProjectTo<ScopeClaimDto>(scope.UserClaims.AsQueryable()));
        }

        /// <summary>Gets the user claim with the specified identifier for the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <param name="id">The identifier of the scope claim.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the scope claim, or <see cref="StatusCodes.Status404NotFound"/> if the scope or claim does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ScopeClaimDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int scopeId, int id)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.UserClaims).FirstOrDefaultAsync(s => s.Id == scopeId);
            var item = scope?.UserClaims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ScopeClaimDto>(item));
        }

        /// <summary>Adds a new user claim to the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <param name="dto">The claim details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created claim, or <see cref="StatusCodes.Status404NotFound"/> if the scope does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ScopeClaimDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int scopeId, [FromBody] SaveScopeClaimDto dto)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.UserClaims).FirstOrDefaultAsync(s => s.Id == scopeId);
            if (scope is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiScopeClaim>(dto);
            entity.ScopeId = scopeId;
            scope.UserClaims.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { scopeId, id = entity.Id }, Mapper.Map<ScopeClaimDto>(entity));
        }

        /// <summary>Deletes the specified user claim from the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <param name="id">The identifier of the scope claim to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the scope or claim does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int scopeId, int id)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.UserClaims).FirstOrDefaultAsync(s => s.Id == scopeId);
            var item = scope?.UserClaims.Find(x => x.Id == id);
            if (item is null) return NotFound();
            scope!.UserClaims.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }

    // ---------------------------------------------------------------------------
    // Properties
    // ---------------------------------------------------------------------------

    /// <summary>Manages custom properties of a scope.</summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/scopes/{scopeId:int}/properties")]
    public class ScopePropertiesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Gets all custom properties for the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the scope properties, or <see cref="StatusCodes.Status404NotFound"/> if the scope does not exist.</returns>
        [HttpGet]
        [ProducesResponseType<ScopePropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int scopeId)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.Properties).FirstOrDefaultAsync(s => s.Id == scopeId);
            if (scope is null) return NotFound();
            return Ok(Mapper.ProjectTo<ScopePropertyDto>(scope.Properties.AsQueryable()));
        }

        /// <summary>Gets the custom property with the specified identifier for the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <param name="id">The identifier of the scope property.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the scope property, or <see cref="StatusCodes.Status404NotFound"/> if the scope or property does not exist.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ScopePropertyDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int scopeId, int id)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.Properties).FirstOrDefaultAsync(s => s.Id == scopeId);
            var item = scope?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            return Ok(Mapper.Map<ScopePropertyDto>(item));
        }

        /// <summary>Adds a new custom property to the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <param name="dto">The property details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created property, or <see cref="StatusCodes.Status404NotFound"/> if the scope does not exist.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ScopePropertyDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int scopeId, [FromBody] SaveScopePropertyDto dto)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.Properties).FirstOrDefaultAsync(s => s.Id == scopeId);
            if (scope is null) return NotFound();
            var entity = Mapper.Map<Duende.IdentityServer.EntityFramework.Entities.ApiScopeProperty>(dto);
            entity.ScopeId = scopeId;
            scope.Properties.Add(entity);
            _ = await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { scopeId, id = entity.Id }, Mapper.Map<ScopePropertyDto>(entity));
        }

        /// <summary>Deletes the specified custom property from the specified scope.</summary>
        /// <param name="scopeId">The identifier of the parent scope.</param>
        /// <param name="id">The identifier of the scope property to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the scope or property does not exist.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int scopeId, int id)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.Properties).FirstOrDefaultAsync(s => s.Id == scopeId);
            var item = scope?.Properties.Find(x => x.Id == id);
            if (item is null) return NotFound();
            scope!.Properties.Remove(item);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
