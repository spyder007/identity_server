using System.Threading.Tasks;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Core.Models;

namespace Spydersoft.Identity.Pages.Home
{
    /// <summary>Displays IdentityServer error details.</summary>
    public class ErrorModel(IIdentityServerInteractionService interaction) : PageModel
    {
        /// <summary>The error view model populated from the IdentityServer error context.</summary>
        public ErrorViewModel View { get; private set; } = new ErrorViewModel();

        /// <summary>The IdentityServer error identifier from the query string.</summary>
        [BindProperty(SupportsGet = true)]
        public string ErrorId { get; set; }

        /// <summary>Loads the error context for the supplied error id.</summary>
        public async Task OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            // retrieve error details from identityserver
            ErrorMessage message = await interaction.GetErrorContextAsync(ErrorId);
            View.Error = message ?? new ErrorMessage { Error = ErrorId };
        }
    }
}
