﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ApiViewModels
{
    public class ApiScopesViewModel : BaseApiCollectionViewModel<ApiScopeViewModel>
    {
    }

    public class ApiScopeViewModel : BaseAdminChildItemViewModel
    {
        public ApiScopeViewModel()
        {
            NewClaim = new ApiScopeClaimViewModel();
            UserClaims = new List<ApiScopeClaimViewModel>();
        }

        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Emphasize")]
        public bool Emphasize { get; set; }

        [Display(Name = "Required")]
        public bool Required { get; set; }

        [Display(Name = "Show in Discovery Document")]
        public bool ShowInDiscoveryDocument { get; set; }

        public List<ApiScopeClaimViewModel> UserClaims { get; set; }

        public ApiScopeClaimViewModel NewClaim { get; set; }
    }

    public class ApiScopeClaimViewModel : BaseAdminChildItemViewModel
    {
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}

