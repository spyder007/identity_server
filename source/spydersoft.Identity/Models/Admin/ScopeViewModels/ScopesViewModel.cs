using System.Linq;

namespace spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class ScopesViewModel
    {
        public IQueryable<ScopeViewModel> Scopes { get; set; }
    }
}