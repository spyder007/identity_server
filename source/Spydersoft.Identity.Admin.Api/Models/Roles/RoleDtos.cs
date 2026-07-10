using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Spydersoft.Identity.Admin.Api.Models.Roles
{
    /// <summary>
    /// Summary representation of an ASP.NET Core Identity role.
    /// </summary>
    public class RoleSummaryDto
    {
        /// <summary>Gets or sets the identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the role name.</summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// Full representation of an ASP.NET Core Identity role.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S2094:Classes should not be empty", Justification = "Intentional distinct API response DTO type; kept separate from RoleSummaryDto to allow detail-specific fields to be added without breaking the summary contract.")]
    public class RoleDto : RoleSummaryDto
    {
    }

    /// <summary>
    /// Data used to create or update an ASP.NET Core Identity role.
    /// </summary>
    public class SaveRoleDto
    {
        /// <summary>Gets or sets the role name.</summary>
        [Required] public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Representation of a claim associated with a role.
    /// </summary>
    public class RoleClaimDto
    {
        /// <summary>Gets or sets the claim type.</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the claim value.</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to create or update a claim on a role.
    /// </summary>
    public class SaveRoleClaimDto
    {
        /// <summary>Gets or sets the claim type.</summary>
        [Required] public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the claim value.</summary>
        public string Value { get; set; } = string.Empty;
    }
}
