using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientClaimsViewModel.
    /// Implements the <see cref="BaseClientCollectionViewModel{TChildViewModel}" />
    /// </summary>
    /// <seealso cref="BaseClientCollectionViewModel{TChildViewModel}" />
    public class ClientClaimsViewModel : BaseClientCollectionViewModel<ClientClaimViewModel>
    {
    }

    /// <summary>
    /// Class ClientClaimViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ClientClaimViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}