using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientPostLogoutRedirectUrisViewModel : BaseClientCollectionViewModel<ClientPostLogoutRedirectUriViewModel>
    {
    }

    public class ClientPostLogoutRedirectUriViewModel : BaseClientChildItemViewModel
    {
        [Required]
        [Url]
        [Display(Name = "Post Logout Redirect URI")]
        public string PostLogoutRedirectUri { get; set; }
    }
}
