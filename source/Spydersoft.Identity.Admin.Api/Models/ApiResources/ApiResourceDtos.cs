using System;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.ApiResources
{
    /// <summary>
    /// Summary representation of an API resource.
    /// </summary>
    public class ApiResourceSummaryDto
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the unique name of the API resource.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name shown for the API resource.</summary>
        public string? DisplayName { get; set; }
        /// <summary>Gets or sets a value indicating whether the API resource is enabled.</summary>
        public bool Enabled { get; set; }
        /// <summary>Gets or sets the date and time the API resource was created.</summary>
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// Detailed representation of an API resource.
    /// </summary>
    public class ApiResourceDto : ApiResourceSummaryDto
    {
        /// <summary>Gets or sets the description of the API resource.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets a value indicating whether the API resource is non-editable.</summary>
        public bool NonEditable { get; set; }
        /// <summary>Gets or sets the date and time the API resource was last accessed.</summary>
        public DateTime LastAccessed { get; set; }
        /// <summary>Gets or sets the date and time the API resource was last updated.</summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// Data used to create or update an API resource.
    /// </summary>
    public class SaveApiResourceDto
    {
        /// <summary>Gets or sets the unique name of the API resource.</summary>
        [Required][StringLength(200, MinimumLength = 2)] public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name shown for the API resource.</summary>
        [StringLength(200, MinimumLength = 2)] public string? DisplayName { get; set; }
        /// <summary>Gets or sets the description of the API resource.</summary>
        [StringLength(1000, MinimumLength = 2)] public string? Description { get; set; }
        /// <summary>Gets or sets a value indicating whether the API resource is enabled.</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>Gets or sets a value indicating whether the API resource is non-editable.</summary>
        public bool? NonEditable { get; set; }
    }
}
