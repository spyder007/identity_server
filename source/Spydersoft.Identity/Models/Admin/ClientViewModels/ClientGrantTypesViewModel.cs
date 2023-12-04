using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientGrantTypesViewModel : BaseClientCollectionViewModel<ClientGrantTypeViewModel>
    {
    }

    public class ClientGrantTypeViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Grant Type")]
        public string GrantType { get; set; }
    }
}