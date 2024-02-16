using System.ComponentModel;
using System.Linq;

namespace Spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    /// <summary>
    /// Class IdentityResourcesViewModel.
    /// </summary>
    public class IdentityResourcesViewModel
    {
        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>The identity resources.</value>
        public IQueryable<IdentityResourceViewModel> IdentityResources { get; set; }

        /// <summary>
        /// Gets or sets the available standard resources.
        /// </summary>
        /// <value>The available standard resources.</value>
        public IQueryable<IdentityResourceViewModel> AvailableStandardResources { get; set; }

        /// <summary>
        /// Gets or sets the selected available resource.
        /// </summary>
        /// <value>The selected available resource.</value>
        [DisplayName("Available Standard Resource Definitions")]
        public string SelectedAvailableResource { get; set; }
    }
}