using System;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.IdentityResources
{
    /// <summary>
    /// Summary view of an identity resource, exposing its core display fields.
    /// </summary>
    public class IdentityResourceSummaryDto
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the unique name of the identity resource.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name shown on the consent screen.</summary>
        public string? DisplayName { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is enabled.</summary>
        public bool Enabled { get; set; }
        /// <summary>Gets or sets the date and time the identity resource was created.</summary>
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// Detailed view of an identity resource, extending the summary with additional configuration.
    /// </summary>
    public class IdentityResourceDto : IdentityResourceSummaryDto
    {
        /// <summary>Gets or sets the description shown on the consent screen.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is emphasized on the consent screen.</summary>
        public bool Emphasize { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is required and cannot be deselected.</summary>
        public bool Required { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is shown in the discovery document.</summary>
        public bool ShowInDiscoveryDocument { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is non-editable.</summary>
        public bool NonEditable { get; set; }
        /// <summary>Gets or sets the date and time the identity resource was last updated.</summary>
        public DateTime? Updated { get; set; }
    }

    /// <summary>
    /// Payload used to create or update an identity resource.
    /// </summary>
    public class SaveIdentityResourceDto
    {
        /// <summary>Gets or sets the unique name of the identity resource.</summary>
        [Required][StringLength(200, MinimumLength = 2)] public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name shown on the consent screen.</summary>
        [StringLength(200, MinimumLength = 2)] public string? DisplayName { get; set; }
        /// <summary>Gets or sets the description shown on the consent screen.</summary>
        [StringLength(1000, MinimumLength = 2)] public string? Description { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is enabled.</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>Gets or sets a value indicating whether the identity resource is emphasized on the consent screen.</summary>
        public bool? Emphasize { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is required and cannot be deselected.</summary>
        public bool? Required { get; set; }
        /// <summary>Gets or sets a value indicating whether the identity resource is shown in the discovery document.</summary>
        public bool ShowInDiscoveryDocument { get; set; } = true;
        /// <summary>Gets or sets a value indicating whether the identity resource is non-editable.</summary>
        public bool? NonEditable { get; set; }
    }
}
