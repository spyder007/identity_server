using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Asp.Versioning;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Admin.Api.Models.Roles;
using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>REST API controller for managing application roles.</summary>
    [ApiVersion("1.0")]
    public class RolesController(RoleManager<ApplicationRole> roleManager, IMapper mapper)
        : BaseAdminApiController(mapper)
    {
        [HttpGet]
        [ProducesResponseType<RoleSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<RoleSummaryDto>(roleManager.Roles));

        [HttpGet("{id}")]
        [ProducesResponseType<RoleDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return role is null ? NotFound() : Ok(Mapper.Map<RoleDto>(role));
        }

        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<RoleDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SaveRoleDto dto)
        {
            var role = new ApplicationRole { Name = dto.Name };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return CreatedAtAction(nameof(GetById), new { id = role.Id }, Mapper.Map<RoleDto>(role));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] SaveRoleDto dto)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();

            role.Name = dto.Name;
            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        // ---- Claims sub-resource ----

        [HttpGet("{id}/claims")]
        [ProducesResponseType<RoleClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClaims(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();
            var claims = await roleManager.GetClaimsAsync(role);
            return Ok(claims.Select(c => new RoleClaimDto { Type = c.Type, Value = c.Value }));
        }

        [HttpPost("{id}/claims")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddClaim(string id, [FromBody] SaveRoleClaimDto dto)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();

            var result = await roleManager.AddClaimAsync(role, new Claim(dto.Type, dto.Value ?? string.Empty));
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        [HttpDelete("{id}/claims/{claimType}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveClaim(string id, string claimType)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();

            var existing = await roleManager.GetClaimsAsync(role);
            var match = existing.FirstOrDefault(c => c.Type == claimType);
            if (match is null) return NotFound();

            var result = await roleManager.RemoveClaimAsync(role, match);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }
    }
}
