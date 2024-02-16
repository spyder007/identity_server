using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.AccountViewModels
{
    /// <summary>
    /// Class LoginWithRecoveryCodeViewModel.
    /// </summary>
    public class LoginWithRecoveryCodeViewModel
    {
        /// <summary>
        /// Gets or sets the recovery code.
        /// </summary>
        /// <value>The recovery code.</value>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}