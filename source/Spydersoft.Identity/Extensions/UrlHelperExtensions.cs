using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Controllers;

namespace Spydersoft.Identity.Extensions
{
    /// <summary>
    /// Class UrlHelperExtensions.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Resets the password callback link.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The code.</param>
        /// <param name="scheme">The scheme.</param>
        /// <returns>System.String.</returns>
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code,
            string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}