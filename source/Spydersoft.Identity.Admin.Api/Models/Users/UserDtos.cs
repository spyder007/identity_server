using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Users
{
    /// <summary>
    /// Summary representation of an ASP.NET Core Identity user.
    /// </summary>
    public class UserSummaryDto
    {
        /// <summary>Gets or sets the unique identifier (GUID string) of the user.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the user name.</summary>
        public string? UserName { get; set; }
        /// <summary>Gets or sets the email address.</summary>
        public string? Email { get; set; }
        /// <summary>Gets or sets a value indicating whether the email address has been confirmed.</summary>
        public bool EmailConfirmed { get; set; }
        /// <summary>Gets or sets the display name.</summary>
        public string? Name { get; set; }
        /// <summary>Gets or sets a value indicating whether two-factor authentication is enabled.</summary>
        public bool TwoFactorEnabled { get; set; }
        /// <summary>Gets or sets a value indicating whether the user can be locked out.</summary>
        public bool LockoutEnabled { get; set; }
        /// <summary>Gets or sets the number of failed access attempts.</summary>
        public int AccessFailedCount { get; set; }
    }

    /// <summary>
    /// Detailed representation of an ASP.NET Core Identity user.
    /// </summary>
    public class UserDto : UserSummaryDto
    {
        /// <summary>Gets or sets the phone number.</summary>
        public string? PhoneNumber { get; set; }
        /// <summary>Gets or sets a value indicating whether the phone number has been confirmed.</summary>
        public bool PhoneNumberConfirmed { get; set; }
    }

    /// <summary>
    /// Data used to update an existing user.
    /// </summary>
    public class SaveUserDto
    {
        /// <summary>Gets or sets the user name.</summary>
        [Required] public string UserName { get; set; } = string.Empty;
        /// <summary>Gets or sets the email address.</summary>
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name.</summary>
        public string? Name { get; set; }
        /// <summary>Gets or sets the phone number.</summary>
        public string? PhoneNumber { get; set; }
        /// <summary>Gets or sets a value indicating whether two-factor authentication is enabled.</summary>
        public bool? TwoFactorEnabled { get; set; }
        /// <summary>Gets or sets a value indicating whether the user can be locked out.</summary>
        public bool? LockoutEnabled { get; set; }
    }

    /// <summary>
    /// Data used to create a new user.
    /// </summary>
    public class CreateUserDto : SaveUserDto
    {
        /// <summary>Gets or sets the initial password.</summary>
        [Required][MinLength(8)] public string Password { get; set; } = string.Empty;
    }

    // ---- Roles ----
    /// <summary>
    /// Represents a role assigned to a user.
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>Gets or sets the role name.</summary>
        public string RoleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to assign a role to a user.
    /// </summary>
    public class AssignUserRoleDto
    {
        /// <summary>Gets or sets the name of the role to assign.</summary>
        [Required] public string RoleName { get; set; } = string.Empty;
    }

    // ---- Claims ----
    /// <summary>
    /// Represents a claim associated with a user.
    /// </summary>
    public class UserClaimDto
    {
        /// <summary>Gets or sets the claim type.</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the claim value.</summary>
        public string Value { get; set; } = string.Empty;
    }
}
