using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.AccountViewModels
{
    /// <summary>
    /// Class ExternalLoginViewModel.
    /// </summary>
    public class ExternalLoginViewModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}