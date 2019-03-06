using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientPropertiesViewModel : BaseClientCollectionViewModel<ClientPropertyViewModel>
    {
    }

    public class ClientPropertyViewModel : BaseClientChildItemViewModel
    {
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Key")]
        public string Key { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}
