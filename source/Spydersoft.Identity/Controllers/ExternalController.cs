﻿using System;
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
using Microsoft.Extensions.Logging;

using Spydersoft.Identity.Attributes;
using Spydersoft.Identity.Exceptions;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// Class ExternalController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<ExternalController> logger) : Controller
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
        /// The interaction
        /// </summary>
        private readonly IIdentityServerInteractionService _interaction = interaction;
        /// <summary>
        /// The events
        /// </summary>
        private readonly IEventService _events = events;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ExternalController> _logger = logger;

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
        /// <exception cref="ArgumentException">invalid return URL, nameof(returnUrl)</exception>
        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (!Url.IsLocalUrl(returnUrl) && !_interaction.IsValidReturnUrl(returnUrl))
            {
                // user might have clicked on a malicious link - should be logged
                throw new ArgumentException("invalid return URL", nameof(returnUrl));
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);

        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                return BadRequest("Cookie Authentication Failed.");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                IEnumerable<string> externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            // lookup our user and external provider info
            (ApplicationUser user, var provider, var providerUserId, IEnumerable<Claim> claims) = await FindUserFromExternalProviderAsync(result);
            user ??= await AutoProvisionUserAsync(provider, providerUserId, claims);

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(user);
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
            Duende.IdentityServer.Models.AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name, true, context?.Client.ClientId));

            if (context != null && context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", returnUrl);
            }

            return Redirect(returnUrl);
        }

        /// <summary>
        /// Find user from external provider as an asynchronous operation.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A Task&lt;(Spydersoft.Identity.Models.Identity.ApplicationUser user, string provider, string providerUserId, System.Collections.Generic.IEnumerable&lt;System.Security.Claims.Claim&gt; claims)&gt; representing the asynchronous operation.</returns>
        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            ClaimsPrincipal externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new IdentityServerException("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            _ = claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            ApplicationUser user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        /// <summary>
        /// Gets the filtered claims.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="email">The email.</param>
        /// <returns>System.Collections.Generic.List&lt;System.Security.Claims.Claim&gt;.</returns>
        private List<Claim> GetFilteredClaims(IEnumerable<Claim> claims, string email)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
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

            // email
            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }
            return filtered;
        }

        /// <summary>
        /// Automatic provision user as an asynchronous operation.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="providerUserId">The provider user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns>A Task&lt;Spydersoft.Identity.Models.Identity.ApplicationUser&gt; representing the asynchronous operation.</returns>
        /// <exception cref="IdentityResultException">createResult</exception>
        /// <exception cref="IdentityResultException">identityResult</exception>
        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            List<Claim> filtered = GetFilteredClaims(claims, email);

            var name = filtered.Find(x => x.Type == JwtClaimTypes.Name)?.Value;

            // If this email already exists in the system, we will link the claims and login to the external provider
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = string.IsNullOrWhiteSpace(email) ? Guid.NewGuid().ToString() : email,
                    Email = email,
                    Name = name
                };
                IdentityResult createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    throw new IdentityResultException(createResult);
                }
            }

            IdentityResult identityResult;
            if (filtered.Count > 0)
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded)
                {
                    throw new IdentityResultException(identityResult);
                }
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            return !identityResult.Succeeded ? throw new IdentityServerException(identityResult.Errors.First().Description) : user;
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        /// <summary>
        /// Processes the login callback.
        /// </summary>
        /// <param name="externalResult">The external result.</param>
        /// <param name="localClaims">The local claims.</param>
        /// <param name="localSignInProps">The local sign in props.</param>
        private static void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            Claim sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
            }
        }
    }
}