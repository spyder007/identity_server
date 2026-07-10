using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.Pages.Account
{
    /// <summary>Confirms a user's email address from an emailed link.</summary>
    [AllowAnonymous]
    public class ConfirmEmailModel(UserManager<ApplicationUser> userManager) : PageModel
    {
        /// <summary>The result message shown to the user.</summary>
        public string Message { get; set; }

        /// <summary>Whether the confirmation succeeded.</summary>
        public bool Succeeded { get; set; }

        /// <summary>Validates the confirmation token and updates the user's email status.</summary>
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await userManager.ConfirmEmailAsync(user, code);
            Succeeded = result.Succeeded;
            Message = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return Page();
        }
    }
}