using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    /// <summary>
    /// Class IdentityResourceClaimsViewModel.
    /// Implements the <see cref="IdentityResourceViewModels.BaseIdentityResourceCollectionViewModel{IdentityResourceViewModels.IdentityResourceClaimViewModel}" />
    /// </summary>
    /// <seealso cref="IdentityResourceViewModels.BaseIdentityResourceCollectionViewModel{IdentityResourceViewModels.IdentityResourceClaimViewModel}" />
    public class IdentityResourceClaimsViewModel : BaseIdentityResourceCollectionViewModel<IdentityResourceClaimViewModel>
    {
    }

    /// <summary>
    /// Class IdentityResourceClaimViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class IdentityResourceClaimViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}