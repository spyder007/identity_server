using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Models.ManageViewModels;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Generates a fresh set of two-factor recovery codes.</summary>
    public class GenerateRecoveryCodesModel(
        UserManager<ApplicationUser> userManager,
        ILogger<GenerateRecoveryCodesModel> logger) : PageModel
    {
        /// <summary>The generated recovery codes.</summary>
        public GenerateRecoveryCodesViewModel View { get; set; }

        /// <summary>Generates new recovery codes for a 2FA-enabled account.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await userManager.GetUserAsync(User)
                ?? throw new ObjectLoadException("user", userManager.GetUserId(User));

            if (!user.TwoFactorEnabled)
            {
                return BadRequest("Multifactor Authentication Unavailable.");
            }

            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            View = new GenerateRecoveryCodesViewModel { RecoveryCodes = [.. recoveryCodes] };

            logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);
            return Page();
        }
    }
}