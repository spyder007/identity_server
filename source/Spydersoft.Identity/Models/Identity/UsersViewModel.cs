using System.Linq;

namespace Spydersoft.Identity.Models.Identity
{
    /// <summary>
    /// Class UsersViewModel.
    /// </summary>
    public class UsersViewModel
    {
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public IQueryable<ApplicationUser> Users { get; set; }
    }
}