using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Core.Exceptions;
using Spydersoft.Identity.Core.Models.Identity;

namespace Spydersoft.Identity.Pages.External
{
    /// <summary>
    /// Handles the callback from an external authentication provider: finds or
    /// auto-provisions the local user and issues the local sign-in cookie.
    /// </summary>
    [AllowAnonymous]
    public class CallbackModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<CallbackModel> logger) : PageModel
    {
        /// <summary>Processes the external authentication result and signs the user in.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            // read external identity from the temporary cookie
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                return BadRequest("Cookie Authentication Failed.");
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                IEnumerable<string> externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                logger.LogDebug("External claims: {Claims}", externalClaims);
            }

            // lookup our user and external provider info
            (ApplicationUser user, var provider, var providerUserId, IEnumerable<Claim> claims) = await FindUserFromExternalProviderAsync(result);
            user ??= await AutoProvisionUserAsync(provider, providerUserId, claims);

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id;

            var isuser = new IdentityServerUser(user.Id)
            {
                DisplayName = name,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            Duende.IdentityServer.Models.AuthorizationRequest context = await interaction.GetAuthorizationContextAsync(returnUrl);
            await events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name, true, context?.Client.ClientId));

            if (context != null && Spydersoft.Identity.Extensions.Extensions.IsNativeClient(context))
            {
                // The client is native, so this change in how to return the response is for better UX.
                return RedirectToPage("/Redirect/Index", new { redirectUri = returnUrl });
            }

            return Redirect(returnUrl);
        }

        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            ClaimsPrincipal externalUser = result.Principal;

            Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new IdentityServerException("Unknown userid");

            var claims = externalUser.Claims.ToList();
            _ = claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            ApplicationUser user = await userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private static List<Claim> GetFilteredClaims(IEnumerable<Claim> claims, string email)
        {
            var filtered = new List<Claim>();

            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

                if (first != null && last != null)
                {
                    name = first + " " + last;
                }
                else if (first != null)
                {
                    name = first;
                }
                else if (last != null)
                {
                    name = last;
                }
                filtered.Add(new Claim(JwtClaimTypes.Name, name ?? "Unknown User"));
            }

            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }
            return filtered;
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            List<Claim> filtered = GetFilteredClaims(claims, email);

            var name = filtered.Find(x => x.Type == JwtClaimTypes.Name)?.Value;

            ApplicationUser user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = string.IsNullOrWhiteSpace(email) ? Guid.NewGuid().ToString() : email,
                    Email = email,
                    Name = name
                };
                IdentityResult createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    throw new IdentityResultException(createResult);
                }
            }

            IdentityResult identityResult;
            if (filtered.Count > 0)
            {
                identityResult = await userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded)
                {
                    throw new IdentityResultException(identityResult);
                }
            }

            identityResult = await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            return !identityResult.Succeeded ? throw new IdentityServerException(identityResult.Errors.First().Description) : user;
        }

        private static void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            Claim sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
            }
        }
    }
}