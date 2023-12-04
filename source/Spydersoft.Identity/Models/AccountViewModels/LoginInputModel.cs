using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.AccountViewModels
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Login")]
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}