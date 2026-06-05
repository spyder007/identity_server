using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Extensions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Models.ManageViewModels;

namespace Spydersoft.Identity.Pages.Manage
{
    /// <summary>User profile page (email / phone).</summary>
    public class IndexModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
    {
        /// <summary>The profile form.</summary>
        [BindProperty]
        public IndexViewModel Input { get; set; }

        /// <summary>Status message shown after a profile action.</summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>Loads the current user's profile.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await ValidateContextUser();
            Input = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneConfirmed = user.PhoneNumberConfirmed,
                StatusMessage = StatusMessage
            };
            return Page();
        }

        /// <summary>Saves email / phone changes.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await ValidateContextUser();

            if (Input.Email != user.Email)
            {
                IdentityResult setEmailResult = await userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new IdentityResultException(setEmailResult);
                }
            }

            if (Input.PhoneNumber != user.PhoneNumber)
            {
                IdentityResult setPhoneResult = await userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new IdentityResultException(setPhoneResult);
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        /// <summary>Sends an email confirmation link.</summary>
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await ValidateContextUser();

            var userId = await userManager.GetUserIdAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null,
                values: new { userId, code }, protocol: Request.Scheme);
            await emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }

        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", userManager.GetUserId(User));
        }
    }
}