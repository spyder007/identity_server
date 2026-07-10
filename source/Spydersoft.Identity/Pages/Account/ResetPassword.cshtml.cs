using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.AccountViewModels;

namespace Spydersoft.Identity.Pages.Account
{
    /// <summary>Resets a password using an emailed reset token.</summary>
    [AllowAnonymous]
    public class ResetPasswordModel(UserManager<ApplicationUser> userManager) : PageModel
    {
        /// <summary>The reset form (email, password, confirmation, code).</summary>
        [BindProperty]
        public ResetPasswordViewModel Input { get; set; }

        /// <summary>Renders the reset form, capturing the reset token from the query string.</summary>
        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), "A code must be supplied for password reset.");
            }

            Input = new ResetPasswordViewModel { Code = code };
            return Page();
        }

        /// <summary>Performs the password reset.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            IdentityResult result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            ModelState.AddErrors(result);
            return Page();
        }
    }
}