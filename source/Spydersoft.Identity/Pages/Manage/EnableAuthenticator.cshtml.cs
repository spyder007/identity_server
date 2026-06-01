using System;
using System.Text;
using System.Text.Encodings.Web;
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
    /// <summary>Sets up an authenticator app (TOTP) and enables two-factor authentication.</summary>
    public class EnableAuthenticatorModel(
        UserManager<ApplicationUser> userManager,
        ILogger<EnableAuthenticatorModel> logger,
        UrlEncoder urlEncoder) : PageModel
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
#pragma warning restore S1075

        /// <summary>The authenticator setup form (code, shared key, URI).</summary>
        [BindProperty]
        public EnableAuthenticatorViewModel Input { get; set; }

        /// <summary>Generates the shared key and QR code URI.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await ValidateContextUser();
            await LoadSharedKeyAndQrCodeAsync(user);
            return Page();
        }

        /// <summary>Verifies the authenticator code and enables 2FA.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await ValidateContextUser();

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeAsync(user);
                return Page();
            }

            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
                user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeAsync(user);
                return Page();
            }

            _ = await userManager.SetTwoFactorEnabledAsync(user, true);
            logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToPage("./GenerateRecoveryCodes");
        }

        private async Task LoadSharedKeyAndQrCodeAsync(ApplicationUser user)
        {
            var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                _ = await userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            }

            Input = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };
        }

        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                _ = result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                _ = result.Append(unformattedKey[currentPosition..]);
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                urlEncoder.Encode("Spydersoft.Identity"),
                urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", userManager.GetUserId(User));
        }
    }
}
