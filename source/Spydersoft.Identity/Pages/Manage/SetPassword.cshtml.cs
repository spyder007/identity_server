using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.ManageViewModels;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>Sets a password for an external-login user that has none.</summary>
    public class SetPasswordModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : PageModel
    {
        /// <summary>The set-password form.</summary>
        [BindProperty]
        public SetPasswordViewModel Input { get; set; }

        /// <summary>Status message shown after the password is set.</summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>Shows the form, redirecting to ChangePassword when a password already exists.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await ValidateContextUser();
            if (await userManager.HasPasswordAsync(user))
            {
                return RedirectToPage("./ChangePassword");
            }

            Input = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return Page();
        }

        /// <summary>Adds the password to the account.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await ValidateContextUser();

            IdentityResult result = await userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddErrors(result);
                return Page();
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToPage();
        }

        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", userManager.GetUserId(User));
        }
    }
}