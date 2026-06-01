using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Constants;
using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Models.ManageViewModels;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Manages the user's linked external login providers.</summary>
    public class ExternalLoginsModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : PageModel
    {
        /// <summary>The current and available external logins.</summary>
        public ExternalLoginsViewModel View { get; set; }

        /// <summary>Status message shown after a link/unlink action.</summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>Loads the linked and available external logins.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            await BuildViewAsync();
            return Page();
        }

        /// <summary>Starts linking a new external provider.</summary>
        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        /// <summary>Handles the provider callback and adds the login to the account.</summary>
        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            ApplicationUser user = await ValidateContextUser();

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync(user.Id)
                ?? throw new ObjectLoadException("externallogininfo", user.Id);
            IdentityResult result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new IdentityResultException(result);
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            StatusMessage = "The external login was added.";
            return RedirectToPage();
        }

        /// <summary>Removes a linked external provider.</summary>
        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            ApplicationUser user = await ValidateContextUser();

            IdentityResult result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                throw new IdentityResultException(result);
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToPage();
        }

        private async Task BuildViewAsync()
        {
            ApplicationUser user = await ValidateContextUser();
            View = new ExternalLoginsViewModel { CurrentLogins = await userManager.GetLoginsAsync(user) };
            View.OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => View.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            View.ShowRemoveButton = await userManager.HasPasswordAsync(user) || View.CurrentLogins.Count > 1;
            View.StatusMessage = StatusMessage;
        }

        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", userManager.GetUserId(User));
        }
    }
}
