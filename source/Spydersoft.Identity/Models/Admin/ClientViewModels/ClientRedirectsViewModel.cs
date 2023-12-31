﻿using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
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