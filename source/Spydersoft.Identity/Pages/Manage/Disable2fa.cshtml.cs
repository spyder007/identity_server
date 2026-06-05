using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Confirms and performs disabling of two-factor authentication.</summary>
    public class Disable2FaModel(
        UserManager<ApplicationUser> userManager,
        ILogger<Disable2FaModel> logger) : PageModel
    {
        /// <summary>Shows the disable-2FA warning, validating 2FA is currently enabled.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await ValidateContextUser();
            return !user.TwoFactorEnabled
                ? throw new IdentityServerException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.")
                : Page();
        }

        /// <summary>Disables two-factor authentication.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await ValidateContextUser();

            IdentityResult result = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                throw new IdentityResultException(result);
            }

            logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToPage("./TwoFactorAuthentication");
        }

        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", userManager.GetUserId(User));
        }
    }
}