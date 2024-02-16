using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourceClaimsViewModel.
    /// Implements the <see cref="ApiResourceViewModels.BaseApiResourceCollectionViewModel{ApiResourceViewModels.ApiResourceClaimViewModel}" />
    /// </summary>
    /// <seealso cref="ApiResourceViewModels.BaseApiResourceCollectionViewModel{ApiResourceViewModels.ApiResourceClaimViewModel}" />
    public class ApiResourceClaimsViewModel : BaseApiResourceCollectionViewModel<ApiResourceClaimViewModel>
    {
    }

    /// <summary>
    /// Class ApiResourceClaimViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ApiResourceClaimViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}