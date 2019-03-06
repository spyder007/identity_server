using System.Linq;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class IdentityResourcesViewModel
    {
        public IQueryable<IdentityResourceViewModel> IdentityResources { get; set; }
    }
}