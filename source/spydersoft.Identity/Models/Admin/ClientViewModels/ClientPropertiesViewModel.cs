﻿using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientPropertiesViewModel : BaseClientCollectionViewModel<ClientPropertyViewModel>
    {
    }

    public class ClientPropertyViewModel : BaseAdminChildItemViewModel
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