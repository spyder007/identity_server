using System.ComponentModel.DataAnnotations;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientClaimsViewModel : BaseClientCollectionViewModel<ClientClaimViewModel>
    {
    }

    public class ClientClaimViewModel : BaseClientChildItemViewModel
    {
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Value")]
        public string Value { get; set; }

        public int Id { get; set; }
    }
}