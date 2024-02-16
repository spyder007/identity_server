using System.Linq;

namespace Spydersoft.Identity.Models.Identity
{
    /// <summary>
    /// Class ApplicationRolesViewModel.
    /// </summary>
    public class ApplicationRolesViewModel
    {
        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public IQueryable<ApplicationRole> Roles { get; set; }
    }
}