﻿using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientPostLogoutRedirectUrisViewModel : BaseClientCollectionViewModel<ClientPostLogoutRedirectUriViewModel>
    {
    }

    public class ClientPostLogoutRedirectUriViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [Url]
        [Display(Name = "Post Logout Redirect URI")]
        public string PostLogoutRedirectUri { get; set; }
    }
}