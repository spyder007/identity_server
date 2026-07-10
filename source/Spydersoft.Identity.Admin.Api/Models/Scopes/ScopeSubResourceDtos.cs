using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Scopes
{
    // ---- Claims ----
    /// <summary>
    /// Representation of a user claim associated with an API scope.
    /// </summary>
    public class ScopeClaimDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the scope that owns this claim.</summary>
        public int ScopeId { get; set; }
        /// <summary>Gets or sets the claim type.</summary>
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to create or update a user claim on an API scope.
    /// </summary>
    public class SaveScopeClaimDto
    {
        /// <summary>Gets or sets the claim type.</summary>
        [Required][StringLength(200, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
    }

    // ---- Properties ----
    /// <summary>
    /// Representation of a custom property (key/value pair) associated with an API scope.
    /// </summary>
    public class ScopePropertyDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the scope that owns this property.</summary>
        public int ScopeId { get; set; }
        /// <summary>Gets or sets the property key.</summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the property value.</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to create or update a custom property on an API scope.
    /// </summary>
    public class SaveScopePropertyDto
    {
        /// <summary>Gets or sets the property key.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the property value.</summary>
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }
}
