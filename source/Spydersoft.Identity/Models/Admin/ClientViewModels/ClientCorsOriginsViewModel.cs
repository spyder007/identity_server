using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientCorsOriginsViewModel.
    /// Implements the <see cref="ClientViewModels.BaseClientCollectionViewModel{ClientViewModels.ClientCorsOriginViewModel}" />
    /// </summary>
    /// <seealso cref="ClientViewModels.BaseClientCollectionViewModel{ClientViewModels.ClientCorsOriginViewModel}" />
    public class ClientCorsOriginsViewModel : BaseClientCollectionViewModel<ClientCorsOriginViewModel>
    {
    }

    /// <summary>
    /// Class ClientCorsOriginViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ClientCorsOriginViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Origin")]
        public string Origin { get; set; }
    }
}