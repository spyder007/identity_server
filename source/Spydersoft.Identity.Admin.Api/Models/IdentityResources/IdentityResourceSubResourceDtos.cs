using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.IdentityResources
{
    // ---- Claims ----
    public class IdentityResourceClaimDto : BaseApiDto
    {
        public int IdentityResourceId { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class SaveIdentityResourceClaimDto
    {
        [Required][StringLength(200, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
    }

    // ---- Properties ----
    public class IdentityResourcePropertyDto : BaseApiDto
    {
        public int IdentityResourceId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SaveIdentityResourcePropertyDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }
}
