using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.IdentityResources
{
    // ---- Claims ----
    /// <summary>
    /// Represents a user claim type associated with an identity resource.
    /// </summary>
    public class IdentityResourceClaimDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the owning identity resource.</summary>
        public int IdentityResourceId { get; set; }
        /// <summary>Gets or sets the user claim type.</summary>
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payload used to create or update an identity resource user claim.
    /// </summary>
    public class SaveIdentityResourceClaimDto
    {
        /// <summary>Gets or sets the user claim type.</summary>
        [Required][StringLength(200, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
    }

    // ---- Properties ----
    /// <summary>
    /// Represents a key/value property associated with an identity resource.
    /// </summary>
    public class IdentityResourcePropertyDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the owning identity resource.</summary>
        public int IdentityResourceId { get; set; }
        /// <summary>Gets or sets the property key.</summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the property value.</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payload used to create or update an identity resource property.
    /// </summary>
    public class SaveIdentityResourcePropertyDto
    {
        /// <summary>Gets or sets the property key.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the property value.</summary>
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }
}
