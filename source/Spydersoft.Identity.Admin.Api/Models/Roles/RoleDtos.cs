using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Roles
{
    public class RoleSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
    }

    public class RoleDto : RoleSummaryDto
    {
    }

    public class SaveRoleDto
    {
        [Required] public string Name { get; set; } = string.Empty;
    }

    public class RoleClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SaveRoleClaimDto
    {
        [Required] public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
