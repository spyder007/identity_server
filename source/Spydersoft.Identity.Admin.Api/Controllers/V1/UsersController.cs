using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Asp.Versioning;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Admin.Api.Models.Users;
using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>REST API controller for managing application users.</summary>
    [ApiVersion("1.0")]
    public class UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper)
        : BaseAdminApiController(mapper)
    {
        /// <summary>Gets a summary list of all users.</summary>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the collection of user summaries.</returns>
        [HttpGet]
        [ProducesResponseType<UserSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
            => Ok(Mapper.ProjectTo<UserSummaryDto>(userManager.Users));

        /// <summary>Gets a single user by their identifier.</summary>
        /// <param name="id">The GUID string identifier of the user.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the user, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return user is null ? NotFound() : Ok(Mapper.Map<UserDto>(user));
        }

        /// <summary>Creates a new user.</summary>
        /// <param name="dto">The user details, including the initial password, to create.</param>
        /// <returns>A <see cref="StatusCodes.Status201Created"/> response containing the created user, or <see cref="StatusCodes.Status400BadRequest"/> if validation fails.</returns>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var user = Mapper.Map<ApplicationUser>(dto);
            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, Mapper.Map<UserDto>(user));
        }

        /// <summary>Updates an existing user.</summary>
        /// <param name="id">The GUID string identifier of the user to update.</param>
        /// <param name="dto">The updated user details.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, <see cref="StatusCodes.Status400BadRequest"/> if validation fails, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] SaveUserDto dto)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            _ = Mapper.Map(dto, user);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        /// <summary>Deletes a user.</summary>
        /// <param name="id">The GUID string identifier of the user to delete.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        // ---- Roles sub-resource ----

        /// <summary>Gets the roles assigned to a user.</summary>
        /// <param name="id">The GUID string identifier of the user.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the user's roles, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpGet("{id}/roles")]
        [ProducesResponseType<UserRoleDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoles(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            IList<string> roles = await userManager.GetRolesAsync(user);
            return Ok(roles.Select(r => new UserRoleDto { RoleName = r }));
        }

        /// <summary>Assigns a role to a user by role name.</summary>
        /// <param name="id">The GUID string identifier of the user.</param>
        /// <param name="dto">The name of the role to assign.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, <see cref="StatusCodes.Status400BadRequest"/> if the role does not exist or validation fails, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpPost("{id}/roles")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRole(string id, [FromBody] AssignUserRoleDto dto)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (!await roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest($"Role '{dto.RoleName}' does not exist.");

            var result = await userManager.AddToRoleAsync(user, dto.RoleName);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        /// <summary>Removes a role from a user by role name.</summary>
        /// <param name="id">The GUID string identifier of the user.</param>
        /// <param name="roleName">The name of the role to remove.</param>
        /// <returns>A <see cref="StatusCodes.Status204NoContent"/> response on success, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpDelete("{id}/roles/{roleName}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRole(string id, string roleName)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));

            return NoContent();
        }

        // ---- Claims sub-resource ----

        /// <summary>Gets the claims assigned to a user.</summary>
        /// <param name="id">The GUID string identifier of the user.</param>
        /// <returns>A <see cref="StatusCodes.Status200OK"/> response containing the user's claims, or <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.</returns>
        [HttpGet("{id}/claims")]
        [ProducesResponseType<UserClaimDto[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClaims(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            var claims = await userManager.GetClaimsAsync(user);
            return Ok(claims.Select(c => new UserClaimDto { Type = c.Type, Value = c.Value }));
        }
    }
}
