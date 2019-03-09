using System.Linq;

namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourcesViewModel
    {
        public IQueryable<IdentityResourceViewModel> IdentityResources { get; set; }
    }
}