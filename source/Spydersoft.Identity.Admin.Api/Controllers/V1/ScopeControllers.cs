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
        [HttpGet]
        [ProducesResponseType<ScopeSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<ScopeSummaryDto>(dbContext.ApiScopes.AsQueryable()));

        [HttpGet("{id:int}")]
        [ProducesResponseType<ScopeDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var entity = dbContext.ApiScopes.FirstOrDefault(s => s.Id == id);
            return entity is null ? NotFound() : Ok(Mapper.Map<ScopeDto>(entity));
        }

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

        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveScopeDto dto)
        {
            var entity = dbContext.ApiScopes.FirstOrDefault(s => s.Id == id);
            if (entity is null) return NotFound();
            _ = Mapper.Map(dto, entity);
            _ = dbContext.ApiScopes.Update(entity);
            _ = await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = dbContext.ApiScopes.FirstOrDefault(s => s.Id == id);
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
        [HttpGet]
        [ProducesResponseType<ScopeClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int scopeId)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.UserClaims).FirstOrDefaultAsync(s => s.Id == scopeId);
            if (scope is null) return NotFound();
            return Ok(Mapper.ProjectTo<ScopeClaimDto>(scope.UserClaims.AsQueryable()));
        }

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
        [HttpGet]
        [ProducesResponseType<ScopePropertyDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int scopeId)
        {
            var scope = await dbContext.ApiScopes.Include(s => s.Properties).FirstOrDefaultAsync(s => s.Id == scopeId);
            if (scope is null) return NotFound();
            return Ok(Mapper.ProjectTo<ScopePropertyDto>(scope.Properties.AsQueryable()));
        }

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
