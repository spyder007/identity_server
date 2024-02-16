using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientGrantTypesViewModel.
    /// Implements the <see cref="ClientViewModels.BaseClientCollectionViewModel{ClientViewModels.ClientGrantTypeViewModel}" />
    /// </summary>
    /// <seealso cref="ClientViewModels.BaseClientCollectionViewModel{ClientViewModels.ClientGrantTypeViewModel}" />
    public class ClientGrantTypesViewModel : BaseClientCollectionViewModel<ClientGrantTypeViewModel>
    {
    }

    /// <summary>
    /// Class ClientGrantTypeViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ClientGrantTypeViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the type of the grant.
        /// </summary>
        /// <value>The type of the grant.</value>
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Grant Type")]
        public string GrantType { get; set; }
    }
}