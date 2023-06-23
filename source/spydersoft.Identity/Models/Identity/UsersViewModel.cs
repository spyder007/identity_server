using System.Linq;

namespace Spydersoft.Identity.Models.Identity
{
    public class UsersViewModel
    {
        public IQueryable<ApplicationUser> Users { get; set; }
    }
}