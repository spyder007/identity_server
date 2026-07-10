using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.ManageViewModels;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Changes the signed-in user's password.</summary>
    public class ChangePasswordModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<ChangePasswordModel> logger) : PageModel
    {
        /// <summary>The change-password form.</summary>
        [BindProperty]
        public ChangePasswordViewModel Input { get; set; }

        /// <summary>Status message shown after a password change.</summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>Shows the form, redirecting to SetPassword when no password is set.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await ValidateContextUser();
            if (!await userManager.HasPasswordAsync(user))
            {
                return RedirectToPage("./SetPassword");
            }

            Input = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return Page();
        }

        /// <summary>Performs the password change.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await ValidateContextUser();

            IdentityResult result = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddErrors(result);
                return Page();
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }

        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", userManager.GetUserId(User));
        }
    }
}