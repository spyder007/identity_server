using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Models.ManageViewModels;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Two-factor authentication overview.</summary>
    public class TwoFactorAuthenticationModel(UserManager<ApplicationUser> userManager) : PageModel
    {
        /// <summary>The 2FA status model.</summary>
        public TwoFactorAuthenticationViewModel View { get; set; }

        /// <summary>Loads the user's 2FA status.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await userManager.GetUserAsync(User)
                ?? throw new ObjectLoadException("user", userManager.GetUserId(User));

            View = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user),
            };
            return Page();
        }
    }
}
