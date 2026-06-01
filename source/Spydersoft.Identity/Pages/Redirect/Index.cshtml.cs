using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Spydersoft.Identity.Pages.Redirect
{
    /// <summary>
    /// Loading page that auto-redirects to a (typically native-client) URL after an
    /// interactive step completes. Replaces the old <c>LoadingPage</c> controller helper.
    /// </summary>
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        /// <summary>The URL to redirect to.</summary>
        [BindProperty(SupportsGet = true)]
        public string RedirectUri { get; set; }

        /// <summary>Renders the loading page.</summary>
        public void OnGet()
        {
        }
    }
}
