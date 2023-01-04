using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourceClaimsViewModel : BaseIdentityResourceCollectionViewModel<IdentityResourceClaimViewModel>
    {
    }

    public class IdentityResourceClaimViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}