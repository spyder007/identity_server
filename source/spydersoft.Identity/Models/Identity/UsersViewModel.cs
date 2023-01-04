using System.Linq;

namespace spydersoft.Identity.Models.Identity
{
    public class UsersViewModel
    {
        public IQueryable<ApplicationUser> Users { get; set; }
    }
}