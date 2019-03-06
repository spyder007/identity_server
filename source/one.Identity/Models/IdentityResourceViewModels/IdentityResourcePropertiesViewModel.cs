﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class IdentityResourcePropertiesViewModel : BaseIdentityResourceCollectionViewModel<IdentityResourcePropertyViewModel>
    {
    }

    public class IdentityResourcePropertyViewModel : BaseIdentityResourceChildItemViewModel
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
