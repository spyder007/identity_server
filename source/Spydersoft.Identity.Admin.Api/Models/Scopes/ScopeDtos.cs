using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Scopes
{
    /// <summary>
    /// Summary representation of an API scope.
    /// </summary>
    public class ScopeSummaryDto
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the unique name of the scope.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name shown to users.</summary>
        public string? DisplayName { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope is enabled.</summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// Full representation of an API scope, including its detailed settings.
    /// </summary>
    public class ScopeDto : ScopeSummaryDto
    {
        /// <summary>Gets or sets the description of the scope.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope should be emphasized on the consent screen.</summary>
        public bool Emphasize { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope is required and cannot be deselected on the consent screen.</summary>
        public bool Required { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope is shown in the discovery document.</summary>
        public bool ShowInDiscoveryDocument { get; set; }
    }

    /// <summary>
    /// Data used to create or update an API scope.
    /// </summary>
    public class SaveScopeDto
    {
        /// <summary>Gets or sets the unique name of the scope.</summary>
        [Required][StringLength(200, MinimumLength = 2)] public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets the display name shown to users.</summary>
        [StringLength(200, MinimumLength = 2)] public string? DisplayName { get; set; }
        /// <summary>Gets or sets the description of the scope.</summary>
        [StringLength(1000, MinimumLength = 2)] public string? Description { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope is enabled.</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>Gets or sets a value indicating whether the scope should be emphasized on the consent screen.</summary>
        public bool? Emphasize { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope is required and cannot be deselected on the consent screen.</summary>
        public bool? Required { get; set; }
        /// <summary>Gets or sets a value indicating whether the scope is shown in the discovery document.</summary>
        public bool ShowInDiscoveryDocument { get; set; } = true;
    }
}
