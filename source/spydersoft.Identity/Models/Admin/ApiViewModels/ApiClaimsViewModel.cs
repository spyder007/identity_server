using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ApiViewModels
{
    public class ApiClaimsViewModel : BaseApiCollectionViewModel<ApiClaimViewModel>
    {
    }

    public class ApiClaimViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}
