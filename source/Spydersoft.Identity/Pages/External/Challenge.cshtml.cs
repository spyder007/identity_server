using System;

using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Spydersoft.Identity.Pages.External
{
    /// <summary>
    /// Initiates a roundtrip to an external authentication provider.
    /// </summary>
    [AllowAnonymous]
    public class ChallengeModel(IIdentityServerInteractionService interaction) : PageModel
    {
        /// <summary>GET is not used; send the user back to login.</summary>
        public IActionResult OnGet() => RedirectToPage("/Account/Login");

        /// <summary>Starts the external challenge for the given scheme and return URL.</summary>
        public IActionResult OnPost(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (!Url.IsLocalUrl(returnUrl) && !interaction.IsValidReturnUrl(returnUrl))
            {
                // user might have clicked on a malicious link - should be logged
                throw new ArgumentException("invalid return URL", nameof(returnUrl));
            }

            // start challenge and roundtrip the return URL and scheme
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/External/Callback"),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);
        }
    }
}