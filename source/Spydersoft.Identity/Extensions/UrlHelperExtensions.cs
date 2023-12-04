using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Controllers;

namespace Spydersoft.Identity.Extensions
{
    public static class UrlHelperExtensions
    {
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