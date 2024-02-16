using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientRedirectsViewModel.
    /// Implements the <see cref="ClientViewModels.BaseClientCollectionViewModel{ClientViewModels.ClientRedirectViewModel}" />
    /// </summary>
    /// <seealso cref="ClientViewModels.BaseClientCollectionViewModel{ClientViewModels.ClientRedirectViewModel}" />
    public class ClientRedirectsViewModel : BaseClientCollectionViewModel<ClientRedirectViewModel>
    {
    }

    /// <summary>
    /// Class ClientRedirectViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ClientRedirectViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>The redirect URI.</value>
        [Required]
        [Url]
        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }
    }
}