using System.ComponentModel;
using System.Linq;

namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourcesViewModel
    {
        public IQueryable<IdentityResourceViewModel> IdentityResources { get; set; }

        public IQueryable<IdentityResourceViewModel> AvailableStandardResources { get; set; }

        [DisplayName("Available Standard Resource Definitions")]
        public string SelectedAvailableResource { get; set; }
    }
}