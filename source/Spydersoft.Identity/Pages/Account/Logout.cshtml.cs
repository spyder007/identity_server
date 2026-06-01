using System.Threading.Tasks;

using Duende.IdentityModel;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Models.AccountViewModels;

namespace Spydersoft.Identity.Pages.Account
{
    /// <summary>Logout prompt and signed-out confirmation page.</summary>
    [AllowAnonymous]
    public class LogoutModel(
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IEventService events,
        IAuthenticationHandlerProvider authenticationHandler) : PageModel
    {
        /// <summary>The logout identifier supplied by IdentityServer.</summary>
        [BindProperty(SupportsGet = true)]
        public string LogoutId { get; set; }

        /// <summary>Whether to show the confirmation prompt.</summary>
        public bool ShowLogoutPrompt { get; set; } = true;

        /// <summary>Whether the user has been signed out (render the confirmation view).</summary>
        public bool SignedOut { get; set; }

        /// <summary>The signed-out display model.</summary>
        public LoggedOutViewModel LoggedOutView { get; set; }

        /// <summary>Shows the logout prompt, or signs the user out directly if it is safe to do so.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            LogoutViewModel vm = await BuildLogoutViewModelAsync(LogoutId);
            ShowLogoutPrompt = vm.ShowLogoutPrompt;

            return vm.ShowLogoutPrompt ? Page() : await PerformLogoutAsync();
        }

        /// <summary>Handles the logout postback.</summary>
        public Task<IActionResult> OnPostAsync() => PerformLogoutAsync();

        private async Task<IActionResult> PerformLogoutAsync()
        {
            LoggedOutViewModel vm = await BuildLoggedOutViewModelAsync(LogoutId);

            if (User?.Identity?.IsAuthenticated == true)
            {
                await signInManager.SignOutAsync();
                await events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            if (vm.TriggerExternalSignout)
            {
                var url = Url.Page("/Account/Logout", new { logoutId = vm.LogoutId });
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            SignedOut = true;
            LoggedOutView = vm;
            return Page();
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity?.IsAuthenticated != true)
            {
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            LogoutRequest context = await interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            LogoutRequest logout = await interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity?.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp is not null and not Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
                {
                    IAuthenticationHandler handler = await authenticationHandler.GetHandlerAsync(HttpContext, idp);
                    if (handler is IAuthenticationSignOutHandler)
                    {
                        vm.LogoutId ??= await interaction.CreateLogoutContextAsync();
                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}
