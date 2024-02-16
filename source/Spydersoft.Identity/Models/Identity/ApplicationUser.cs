using Microsoft.AspNetCore.Identity;

namespace Spydersoft.Identity.Models.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    /// <summary>
    /// Class ApplicationUser.
    /// Implements the <see cref="IdentityUser" />
    /// </summary>
    /// <seealso cref="IdentityUser" />
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [PersonalData]
        public string Name { get; set; }
    }
}