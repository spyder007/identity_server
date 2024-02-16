using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    /// <summary>
    /// Class ScopeClaimViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ScopeClaimViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}