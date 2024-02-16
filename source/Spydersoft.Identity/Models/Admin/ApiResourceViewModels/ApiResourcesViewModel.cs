using System.Linq;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourcesViewModel.
    /// </summary>
    public class ApiResourcesViewModel
    {
        /// <summary>
        /// Gets or sets the apis.
        /// </summary>
        /// <value>The apis.</value>
        public IQueryable<ApiResourceViewModel> Apis { get; set; }
    }
}