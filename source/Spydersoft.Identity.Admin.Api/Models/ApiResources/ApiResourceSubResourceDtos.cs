using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.ApiResources
{
    // ---- Claims ----
    /// <summary>
    /// Represents a user claim type associated with an API resource.
    /// </summary>
    public class ApiResourceClaimDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the owning API resource.</summary>
        public int ApiResourceId { get; set; }
        /// <summary>Gets or sets the claim type.</summary>
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to create or update an API resource claim.
    /// </summary>
    public class SaveApiResourceClaimDto
    {
        /// <summary>Gets or sets the claim type.</summary>
        [Required][StringLength(200, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
    }

    // ---- Properties ----
    /// <summary>
    /// Represents a custom property (key/value pair) associated with an API resource.
    /// </summary>
    public class ApiResourcePropertyDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the owning API resource.</summary>
        public int ApiResourceId { get; set; }
        /// <summary>Gets or sets the property key.</summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the property value.</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to create or update an API resource property.
    /// </summary>
    public class SaveApiResourcePropertyDto
    {
        /// <summary>Gets or sets the property key.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the property value.</summary>
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }

    // ---- Scopes ----
    /// <summary>
    /// Represents a scope associated with an API resource.
    /// </summary>
    public class ApiResourceScopeDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the owning API resource.</summary>
        public int ApiResourceId { get; set; }
        /// <summary>Gets or sets the scope name.</summary>
        public string Scope { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data used to create or update an API resource scope.
    /// </summary>
    public class SaveApiResourceScopeDto
    {
        /// <summary>Gets or sets the scope name.</summary>
        [Required][StringLength(200, MinimumLength = 1)] public string Scope { get; set; } = string.Empty;
    }

    // ---- Secrets ----
    /// <summary>
    /// Represents a secret associated with an API resource.
    /// </summary>
    public class ApiResourceSecretDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the owning API resource.</summary>
        public int ApiResourceId { get; set; }
        /// <summary>Gets or sets the secret type.</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the description of the secret.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets the expiration of the secret.</summary>
        public string? Expiration { get; set; }
    }

    /// <summary>
    /// Data used to create or update an API resource secret.
    /// </summary>
    public class SaveApiResourceSecretDto
    {
        /// <summary>Gets or sets the secret type.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Type { get; set; } = "SharedSecret";
        /// <summary>Gets or sets the secret value.</summary>
        [Required][StringLength(4000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
        /// <summary>Gets or sets the description of the secret.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets the expiration of the secret.</summary>
        public string? Expiration { get; set; }
    }
}
