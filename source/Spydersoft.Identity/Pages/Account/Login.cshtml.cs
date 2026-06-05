using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.AccountViewModels;

namespace Spydersoft.Identity.Pages.Account
{
    /// <summary>Username/password (and external provider) login page.</summary>
    [AllowAnonymous]
    public class LoginModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider,
        IEventService events) : PageModel
    {
        /// <summary>The posted login credentials.</summary>
        [BindProperty]
        public LoginInputModel Input { get; set; }

        /// <summary>The display model (providers, local-login flag, return url).</summary>
        public LoginViewModel View { get; set; }

        /// <summary>True when the client only allows an external provider but a local session exists.</summary>
        public bool ShowExternalLoginRequired { get; set; }

        /// <summary>Entry point into the login workflow.</summary>
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            View = await BuildLoginViewModelAsync(returnUrl);
            Input = new LoginInputModel { ReturnUrl = returnUrl };

            if (View.IsExternalLoginOnly && (User?.Identity?.IsAuthenticated == true))
            {
                ShowExternalLoginRequired = true;
            }

            return Page();
        }

        /// <summary>Handles postback from username/password login.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            AuthorizationRequest context = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

            if (!ModelState.IsValid)
            {
                View = await BuildLoginViewModelAsync(Input);
                return Page();
            }

            Microsoft.AspNetCore.Identity.SignInResult result =
                await signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberLogin, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                View = await BuildLoginViewModelAsync(Input);
                return Page();
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { rememberMe = Input.RememberLogin, returnUrl = Input.ReturnUrl });
            }

            ApplicationUser user = await userManager.FindByNameAsync(Input.Username);
            await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    return RedirectToPage("/Redirect/Index", new { redirectUri = Input.ReturnUrl });
                }

                return Redirect(Input.ReturnUrl);
            }

            if (Url.IsLocalUrl(Input.ReturnUrl))
            {
                return Redirect(Input.ReturnUrl);
            }
            if (string.IsNullOrEmpty(Input.ReturnUrl))
            {
                return Redirect("~/");
            }

            ModelState.AddModelError("Input.ReturnUrl", "invalid return url");
            View = await BuildLoginViewModelAsync(Input);
            return Page();
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            AuthorizationRequest context = await interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = [new ExternalProvider { AuthenticationScheme = context.IdP }];
                }

                return vm;
            }

            IEnumerable<AuthenticationScheme> schemes = await schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                Client client = await clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Count > 0)
                    {
                        providers = [.. providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme))];
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = [.. providers]
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            LoginViewModel vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }
    }
}