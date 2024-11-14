using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Constants;
using Spydersoft.Identity.Exceptions;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.Identity;
using Spydersoft.Identity.Models.ManageViewModels;
using Spydersoft.Identity.Services;

namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// Class ManageController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [Authorize]
    public class ManageController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IEmailSender emailSender,
      ILogger<ManageController> logger,
      UrlEncoder urlEncoder) : Controller
    {
        /// <summary>
        /// The user manager
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        /// <summary>
        /// The sign in manager
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        /// <summary>
        /// The email sender
        /// </summary>
        private readonly IEmailSender _emailSender = emailSender;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger = logger;
        /// <summary>
        /// The URL encoder
        /// </summary>
        private readonly UrlEncoder _urlEncoder = urlEncoder;

#pragma warning disable S1075 // URIs should not be hardcoded
        /// <summary>
        /// The authenicator URI format
        /// </summary>
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>The status message.</value>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await ValidateContextUser();

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneConfirmed = user.PhoneNumberConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        /// <summary>
        /// Indexes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        /// <exception cref="IdentityResultException"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await ValidateContextUser();

            var email = user.Email;
            if (model.Email != email)
            {
                IdentityResult setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new IdentityResultException(setEmailResult);
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                IdentityResult setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new IdentityResultException(setPhoneResult);
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sends the verification email.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await ValidateContextUser();

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Action(nameof(AccountController.ConfirmEmail), "Account",
                values: new { userId, code }, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ApplicationUser user = await ValidateContextUser();

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await ValidateContextUser();

            IdentityResult changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                ModelState.AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            ApplicationUser user = await ValidateContextUser();

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await ValidateContextUser();

            IdentityResult addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                ModelState.AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        /// <summary>
        /// Externals the logins.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            ApplicationUser user = await ValidateContextUser();

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        /// <summary>
        /// Links the login.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ExternalLogins));
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// Links the login callback.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// <exception cref="ObjectLoadException">externallogininfo</exception>
        /// <exception cref="IdentityResultException"></exception>
        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            ApplicationUser user = await ValidateContextUser();

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(user.Id) ?? throw new ObjectLoadException("externallogininfo", user.Id);
            IdentityResult result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new IdentityResultException(result);
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        /// <summary>
        /// Removes the login.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        /// <exception cref="IdentityResultException"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new IdentityServerException(Messages.InvalidRequest);
            }

            ApplicationUser user = await ValidateContextUser();

            IdentityResult result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new IdentityResultException(result);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        /// <summary>
        /// Twoes the factor authentication.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            ApplicationUser user = await ValidateContextUser();

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        /// <summary>
        /// Disable2fas the warning.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// <exception cref="IdentityServerException">Unexpected error occured disabling 2FA for user with ID '{user.Id}'.</exception>
        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            ApplicationUser user = await ValidateContextUser();
            return !user.TwoFactorEnabled
                ? throw new IdentityServerException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.")
                : (IActionResult)View(nameof(Disable2fa));
        }

        /// <summary>
        /// Disable2fas this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// <exception cref="IdentityResultException"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            ApplicationUser user = await ValidateContextUser();

            IdentityResult disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new IdentityResultException(disable2faResult);
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        /// <summary>
        /// Enables the authenticator.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            ApplicationUser user = await ValidateContextUser();

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                _ = await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        /// <summary>
        /// Enables the authenticator.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await ValidateContextUser();

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            _ = await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        /// <summary>
        /// Resets the authenticator warning.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        /// <summary>
        /// Resets the authenticator.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            ApplicationUser user = await ValidateContextUser();

            _ = await _userManager.SetTwoFactorEnabledAsync(user, false);
            _ = await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        /// <summary>
        /// Generates the recovery codes.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            ApplicationUser user = await ValidateContextUser();

            if (!user.TwoFactorEnabled)
            {
                return BadRequest("Multifactor Authentication Unavailable.");
            }

            System.Collections.Generic.IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        /// <summary>
        /// Users the claims information.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult UserClaimsInfo()
        {
            return View();
        }

        #region Helpers



        /// <summary>
        /// Formats the key.
        /// </summary>
        /// <param name="unformattedKey">The unformatted key.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Generates the qr code URI.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="unformattedKey">The unformatted key.</param>
        /// <returns>System.String.</returns>
        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("Spydersoft.Identity"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        /// <summary>
        /// Validates the context user.
        /// </summary>
        /// <returns>ApplicationUser.</returns>
        /// <exception cref="ObjectLoadException">user</exception>
        private async Task<ApplicationUser> ValidateContextUser()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return user ?? throw new ObjectLoadException("user", _userManager.GetUserId(User));
        }

        #endregion Helpers
    }
}