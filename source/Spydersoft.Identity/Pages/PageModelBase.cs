using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Constants;

namespace Spydersoft.Identity.Pages
{
    /// <summary>
    /// Base <see cref="PageModel"/> with the shared redirect helpers that the old
    /// MVC <c>BaseController</c> provided.
    /// </summary>
    public abstract class PageModelBase : PageModel
    {
        /// <summary>
        /// Redirects to a local return URL, or home if the URL is not local.
        /// </summary>
        protected IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToPage("/Index");
        }

        /// <summary>
        /// Redirects to the error page, substituting a generic message when model state is invalid.
        /// </summary>
        protected IActionResult GetErrorAction(string errorMessage)
        {
            return RedirectToPage("/Home/Error", new { errorId = ModelState.IsValid ? errorMessage : Messages.InvalidRequest });
        }
    }
}