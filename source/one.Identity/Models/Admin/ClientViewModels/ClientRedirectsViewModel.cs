using System.ComponentModel.DataAnnotations;

namespace one.Identity.Models.Admin.ClientViewModels
{
    public class ClientRedirectsViewModel : BaseClientCollectionViewModel<ClientRedirectViewModel>
    {
    }

    public class ClientRedirectViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [Url]
        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }
    }
}
