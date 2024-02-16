using System;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Spydersoft.Identity.Views.Manage
{
    /// <summary>
    /// Class ManageNavPages.
    /// </summary>
    public static class ManageNavPages
    {
        /// <summary>
        /// Gets the active page key.
        /// </summary>
        /// <value>The active page key.</value>
        public static string ActivePageKey => "ActivePage";

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public static string Index => "Index";

        /// <summary>
        /// Gets the change password.
        /// </summary>
        /// <value>The change password.</value>
        public static string ChangePassword => "ChangePassword";

        /// <summary>
        /// Gets the external logins.
        /// </summary>
        /// <value>The external logins.</value>
        public static string ExternalLogins => "ExternalLogins";

        /// <summary>
        /// Gets the two factor authentication.
        /// </summary>
        /// <value>The two factor authentication.</value>
        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        /// <summary>
        /// Gets the user claims information.
        /// </summary>
        /// <value>The user claims information.</value>
        public static string UserClaimsInfo => "UserClaimsInfo";

        /// <summary>
        /// Indexes the nav class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>System.String.</returns>
        public static string IndexNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, Index);
        }

        /// <summary>
        /// Changes the password nav class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>System.String.</returns>
        public static string ChangePasswordNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, ChangePassword);
        }

        /// <summary>
        /// Externals the logins nav class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>System.String.</returns>
        public static string ExternalLoginsNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, ExternalLogins);
        }

        /// <summary>
        /// Twoes the factor authentication nav class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>System.String.</returns>
        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, TwoFactorAuthentication);
        }

        /// <summary>
        /// Pages the nav class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="page">The page.</param>
        /// <returns>System.String.</returns>
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        /// <summary>
        /// Users the claims information nav class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>System.String.</returns>
        public static string UserClaimsInfoNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, UserClaimsInfo);
        }

        /// <summary>
        /// Adds the active page.
        /// </summary>
        /// <param name="viewData">The view data.</param>
        /// <param name="activePage">The active page.</param>
        public static void AddActivePage(this ViewDataDictionary viewData, string activePage)
        {
            viewData[ActivePageKey] = activePage;
        }
    }
}