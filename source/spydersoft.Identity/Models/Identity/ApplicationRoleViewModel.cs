using System.Linq;
using System.Security.Claims;

namespace spydersoft.Identity.Models.Identity
{
    public class ApplicationRoleViewModel
    {
        public ApplicationRole Role { get; set; }
        public IQueryable<Claim> Claims { get; set; }
        public ClaimModel NewClaim { get; set; }
    }
}