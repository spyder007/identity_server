using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ApiViewModels
{
    public class ApiClaimsViewModel : BaseApiCollectionViewModel<ApiClaimViewModel>
    {
    }

    public class ApiClaimViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Type { get; set; }
    }
}
