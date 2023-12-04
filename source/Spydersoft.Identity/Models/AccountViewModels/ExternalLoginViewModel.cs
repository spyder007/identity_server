using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}