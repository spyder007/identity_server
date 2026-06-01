using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.AccountViewModels;

namespace Spydersoft.Identity.Pages.Account
{
    /// <summary>Requests a password-reset email.</summary>
    [AllowAnonymous]
    public class ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
    {
        /// <summary>The submitted email address.</summary>
        [BindProperty]
        public ForgotPasswordViewModel Input { get; set; }

        /// <summary>Renders the forgot-password form.</summary>
        public void OnGet()
        {
        }

        /// <summary>Sends a password reset link if the account exists and is confirmed.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            await emailSender.SendEmailAsync(Input.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
