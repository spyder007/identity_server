using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Confirms and performs an authenticator key reset.</summary>
    public class ResetAuthenticatorModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetAuthenticatorModel> logger) : PageModel
    {
        /// <summary>Shows the reset-authenticator warning.</summary>
        public void OnGet()
        {
            // No initialization required; the page renders with its default state.
        }

        /// <summary>Resets the authenticator key and disables 2FA.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await userManager.GetUserAsync(User)
                ?? throw new ObjectLoadException("user", userManager.GetUserId(User));

            _ = await userManager.SetTwoFactorEnabledAsync(user, false);
            _ = await userManager.ResetAuthenticatorKeyAsync(user);
            logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToPage("./EnableAuthenticator");
        }
    }
}