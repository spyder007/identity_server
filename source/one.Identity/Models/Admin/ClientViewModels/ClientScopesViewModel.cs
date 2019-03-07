using System.ComponentModel.DataAnnotations;

namespace one.Identity.Models.Admin.ClientViewModels
{
    public class ClientScopesViewModel : BaseClientCollectionViewModel<ClientScopeViewModel>
    {
    }

    public class ClientScopeViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Scope")]
        public string Scope { get; set; }
    }
}
