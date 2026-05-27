using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Scopes
{
    // ---- Claims ----
    public class ScopeClaimDto : BaseApiDto
    {
        public int ScopeId { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class SaveScopeClaimDto
    {
        [Required][StringLength(200, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
    }

    // ---- Properties ----
    public class ScopePropertyDto : BaseApiDto
    {
        public int ScopeId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SaveScopePropertyDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }
}
