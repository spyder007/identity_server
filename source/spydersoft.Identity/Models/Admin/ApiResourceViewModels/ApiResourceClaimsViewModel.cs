using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceClaimsViewModel : BaseApiResourceCollectionViewModel<ApiResourceClaimViewModel>
    {
    }

    public class ApiResourceClaimViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}