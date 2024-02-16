using System.Linq;

namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    /// <summary>
    /// Class ScopesViewModel.
    /// </summary>
    public class ScopesViewModel
    {
        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>The scopes.</value>
        public IQueryable<ScopeViewModel> Scopes { get; set; }
    }
}