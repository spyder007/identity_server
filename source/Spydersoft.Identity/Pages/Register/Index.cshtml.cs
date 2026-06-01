using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Duende.IdentityModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Extensions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.AccountViewModels;

namespace Spydersoft.Identity.Pages.Register
{
    /// <summary>New account registration page.</summary>
    [AllowAnonymous]
    public class IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<IndexModel> logger,
        IEmailSender emailSender) : PageModelBase
    {
        /// <summary>The registration form (name, email, password, confirmation).</summary>
        [BindProperty]
        public RegisterViewModel Input { get; set; }

        /// <summary>The return URL to redirect to after registration.</summary>
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        /// <summary>Renders the registration form.</summary>
        public void OnGet()
        {
        }

        /// <summary>Creates the account, sends a confirmation email, and signs the user in.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                ApplicationUser existingUser = await userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "An account already exists for this email.");
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        Name = Input.Name
                    };
                    IdentityResult result = await userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("User created a new account with password.");
                        var userId = await userManager.GetUserIdAsync(user);
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null,
                            values: new { userId, code }, protocol: Request.Scheme);
                        await emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                        await signInManager.SignInAsync(user, isPersistent: false);
                        _ = await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Name, Input.Name));
                        return RedirectToLocal(ReturnUrl);
                    }
                    ModelState.AddErrors(result);
                }
            }

            return Page();
        }
    }
}
