using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientIdpRestrictionsViewModel : BaseClientCollectionViewModel<ClientIdpRestrictionViewModel>
    {
    }

    public class ClientIdpRestrictionViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Provider")]
        public string Provider { get; set; }
    }
}