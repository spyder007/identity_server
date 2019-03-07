using System.Linq;

namespace one.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourcesViewModel
    {
        public IQueryable<IdentityResourceViewModel> IdentityResources { get; set; }
    }
}