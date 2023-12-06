using System.Linq;

namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class ScopesViewModel
    {
        public IQueryable<ScopeViewModel> Scopes { get; set; }
    }
}