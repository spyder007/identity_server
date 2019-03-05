using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientRedirectsViewModel : BaseClientCollectionViewModel<ClientRedirectViewModel>
    {
    }

    public class ClientRedirectViewModel : BaseClientChildItemViewModel
    {
        [Required]
        [Url]
        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }

        public int Id { get; set; }
    }
}
