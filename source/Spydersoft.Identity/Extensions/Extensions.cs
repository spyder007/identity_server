using System;

using Duende.IdentityServer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Spydersoft.Identity.Models.AccountViewModels;
using Spydersoft.Identity.Models.Consent;

namespace Spydersoft.Identity.Extensions
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if the redirect URI is for a native client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if [is native client] [the specified context]; otherwise, <c>false</c>.</returns>
        public static bool IsNativeClient(this AuthorizationRequest context)
        {
            return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
                   && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }

        /// <summary>
        /// Loadings the page.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <returns>IActionResult.</returns>
        public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
        {
            controller.HttpContext.Response.StatusCode = 200;
            controller.HttpContext.Response.Headers.Location = string.Empty;

            return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
        }

        /// <summary>
        /// Adds the errors.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="result">The result.</param>
        public static void AddErrors(this ModelStateDictionary modelState, IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                modelState.AddModelError(string.Empty, error.Description);
            }
        }

        /// <summary>
        /// Creates the scope view model.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="check">if set to <c>true</c> [check].</param>
        /// <returns>ScopeViewModel.</returns>
        public static ScopeViewModel CreateScopeViewModel(this IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }
    }

}