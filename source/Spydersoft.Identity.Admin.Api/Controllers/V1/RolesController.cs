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
        /// <summary>Gets a summary list of all roles.</summary>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the collection of role summaries.</returns>
        [HttpGet]
        [ProducesResponseType<RoleSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<RoleSummaryDto>(roleManager.Roles));

        /// <summary>Gets a single role by its identifier.</summary>
        /// <param name="id">The unique identifier of the role.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the role, or <see cref="StatusCodes.Status404NotFound"/> if it does not exist.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType<RoleDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return role is null ? NotFound() : Ok(Mapper.Map<RoleDto>(role));
        }

        /// <summary>Creates a new role.</summary>
        /// <param name="dto">The role details to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created role, or <see cref="StatusCodes.Status400BadRequest"/> if validation fails.</returns>
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

        /// <summary>Updates an existing role.</summary>
        /// <param name="id">The unique identifier of the role to update.</param>
        /// <param name="dto">The updated role details.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, <see cref="StatusCodes.Status400BadRequest"/> if validation fails, or <see cref="StatusCodes.Status404NotFound"/> if the role does not exist.</returns>
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

        /// <summary>Deletes a role.</summary>
        /// <param name="id">The unique identifier of the role to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the role does not exist.</returns>
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

        /// <summary>Gets the claims assigned to a role.</summary>
        /// <param name="id">The unique identifier of the role.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the role's claims, or <see cref="StatusCodes.Status404NotFound"/> if the role does not exist.</returns>
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

        /// <summary>Adds a claim to a role.</summary>
        /// <param name="id">The unique identifier of the role.</param>
        /// <param name="dto">The claim type and value to add.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, <see cref="StatusCodes.Status400BadRequest"/> if validation fails, or <see cref="StatusCodes.Status404NotFound"/> if the role does not exist.</returns>
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

        /// <summary>Removes a claim of the specified type from a role.</summary>
        /// <param name="id">The unique identifier of the role.</param>
        /// <param name="claimType">The type of the claim to remove.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the role or claim does not exist.</returns>
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
