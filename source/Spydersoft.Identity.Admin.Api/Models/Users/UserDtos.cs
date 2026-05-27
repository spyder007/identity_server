using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Users
{
    public class UserSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? Name { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }

    public class UserDto : UserSummaryDto
    {
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }

    public class SaveUserDto
    {
        [Required] public string UserName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
    }

    public class CreateUserDto : SaveUserDto
    {
        [Required][MinLength(8)] public string Password { get; set; } = string.Empty;
    }

    // ---- Roles ----
    public class UserRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
    }

    public class AssignUserRoleDto
    {
        [Required] public string RoleName { get; set; } = string.Empty;
    }

    // ---- Claims ----
    public class UserClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
