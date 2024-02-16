using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using IdentityModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Attributes;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.AccountViewModels;
using Spydersoft.Identity.Models.Identity;
using Spydersoft.Identity.Services;

/// <summary>
/// The Controllers namespace.
/// </summary>
namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// Class RegisterController.
    /// Implements the <see cref="BaseController" />
    /// </summary>
    /// <seealso cref="BaseController" />
    [SecurityHeaders]
    [AllowAnonymous]
    public class RegisterController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterController> logger,
        IMapper mapper,
        IEmailSender emailSender) : BaseController(mapper)
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
        private readonly ILogger<RegisterController> _logger = logger;

        /// <summary>
        /// Indexes the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Indexes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", $"An account already exists for this email.");
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Name = model.Name
                    };
                    IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action(nameof(AccountController.ConfirmEmail), "Account",
                            new { userId, code }, Request.Scheme);
                        await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created a new account with password.");
                        _ = await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Name, model.Name));
                        return RedirectToLocal(returnUrl);
                    }
                    ModelState.AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}