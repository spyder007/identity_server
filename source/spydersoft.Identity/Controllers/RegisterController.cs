using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using spydersoft.Identity.Attributes;
using spydersoft.Identity.Extensions;
using spydersoft.Identity.Models.AccountViewModels;
using spydersoft.Identity.Models.Identity;
using spydersoft.Identity.Services;

/// <summary>
/// The Controllers namespace.
/// </summary>
namespace spydersoft.Identity.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class RegisterController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterController> logger,
            IMapper mapper,
            IEmailSender emailSender) : base(mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

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