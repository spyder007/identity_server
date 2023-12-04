using Microsoft.AspNetCore.Identity;

namespace Spydersoft.Identity.Models.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
    }
}