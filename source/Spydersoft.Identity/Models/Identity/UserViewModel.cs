using System.Linq;

namespace Spydersoft.Identity.Models.Identity
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public string Password { get; set; }
        public bool IsNewUser { get; set; }

        public IQueryable<string> Roles { get; set; }
        public IQueryable<string> AvailableRoles { get; set; }
        public string SelectedAvailableRole { get; set; }

        public IQueryable<ClaimModel> Claims { get; set; }
        public ClaimModel NewClaim { get; set; }

    }
}