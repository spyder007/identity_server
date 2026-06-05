using Duende.IdentityServer.Models;

namespace Spydersoft.Identity.Core.Models
{
    /// <summary>
    /// Class ErrorViewModel.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public ErrorMessage Error { get; set; } = new();
    }
}
