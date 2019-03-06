using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class IdentityResourceClaimsViewModel : BaseIdentityResourceCollectionViewModel<IdentityResourceClaimViewModel>
    {
    }

    public class IdentityResourceClaimViewModel : BaseIdentityResourceChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}
