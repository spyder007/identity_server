using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Models.AccountViewModels;

namespace Spydersoft.Identity.Pages.Account
{
    /// <summary>Two-factor authenticator code entry during login.</summary>
    [AllowAnonymous]
    public class LoginWith2faModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginWith2faModel> logger) : PageModel
    {
        /// <summary>The submitted authenticator code and options.</summary>
        [BindProperty]
        public LoginWith2FaViewModel Input { get; set; }

        /// <summary>Verifies the user completed the first login step and renders the 2FA form.</summary>
        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            _ = await signInManager.GetTwoFactorAuthenticationUserAsync()
                ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

            Input = new LoginWith2FaViewModel { RememberMe = rememberMe, ReturnUrl = returnUrl };
            return Page();
        }

        /// <summary>Validates the authenticator code and completes sign-in.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await signInManager.GetTwoFactorAuthenticationUserAsync()
                ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            Microsoft.AspNetCore.Identity.SignInResult result =
                await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, Input.RememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return !string.IsNullOrEmpty(Input.ReturnUrl) ? Redirect(Input.ReturnUrl) : Redirect("~/");
            }

            if (result.IsLockedOut)
            {
                logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }

            logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }
}
