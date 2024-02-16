using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientPostLogoutRedirectUrisViewModel.
    /// Implements the <see cref="BaseClientCollectionViewModel{TChildViewModel}" />
    /// </summary>
    /// <seealso cref="BaseClientCollectionViewModel{TChildViewModel}" />
    public class ClientPostLogoutRedirectUrisViewModel : BaseClientCollectionViewModel<ClientPostLogoutRedirectUriViewModel>
    {
    }

    /// <summary>
    /// Class ClientPostLogoutRedirectUriViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ClientPostLogoutRedirectUriViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the post logout redirect URI.
        /// </summary>
        /// <value>The post logout redirect URI.</value>
        [Required]
        [Url]
        [Display(Name = "Post Logout Redirect URI")]
        public string PostLogoutRedirectUri { get; set; }
    }
}