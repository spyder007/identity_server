using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.ApiResources
{
    // ---- Claims ----
    public class ApiResourceClaimDto : BaseApiDto
    {
        public int ApiResourceId { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class SaveApiResourceClaimDto
    {
        [Required][StringLength(200, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
    }

    // ---- Properties ----
    public class ApiResourcePropertyDto : BaseApiDto
    {
        public int ApiResourceId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SaveApiResourcePropertyDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }

    // ---- Scopes ----
    public class ApiResourceScopeDto : BaseApiDto
    {
        public int ApiResourceId { get; set; }
        public string Scope { get; set; } = string.Empty;
    }

    public class SaveApiResourceScopeDto
    {
        [Required][StringLength(200, MinimumLength = 1)] public string Scope { get; set; } = string.Empty;
    }

    // ---- Secrets ----
    public class ApiResourceSecretDto : BaseApiDto
    {
        public int ApiResourceId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Expiration { get; set; }
    }

    public class SaveApiResourceSecretDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Type { get; set; } = "SharedSecret";
        [Required][StringLength(4000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Expiration { get; set; }
    }
}
