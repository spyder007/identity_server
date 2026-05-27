using System;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.IdentityResources
{
    public class IdentityResourceSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public bool Enabled { get; set; }
        public DateTime Created { get; set; }
    }

    public class IdentityResourceDto : IdentityResourceSummaryDto
    {
        public string? Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public bool NonEditable { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class SaveIdentityResourceDto
    {
        [Required][StringLength(200, MinimumLength = 2)] public string Name { get; set; } = string.Empty;
        [StringLength(200, MinimumLength = 2)] public string? DisplayName { get; set; }
        [StringLength(1000, MinimumLength = 2)] public string? Description { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public bool NonEditable { get; set; }
    }
}
