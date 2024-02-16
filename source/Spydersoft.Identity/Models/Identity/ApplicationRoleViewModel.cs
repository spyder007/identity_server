using System.Linq;
using System.Security.Claims;

namespace Spydersoft.Identity.Models.Identity
{
    /// <summary>
    /// Class ApplicationRoleViewModel.
    /// </summary>
    public class ApplicationRoleViewModel
    {
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        public ApplicationRole Role { get; set; }
        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>The claims.</value>
        public IQueryable<Claim> Claims { get; set; }
        /// <summary>
        /// Creates new claim.
        /// </summary>
        /// <value>The new claim.</value>
        public ClaimModel NewClaim { get; set; }
    }
}