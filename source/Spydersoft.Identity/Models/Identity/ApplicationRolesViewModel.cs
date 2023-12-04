using System.Linq;

namespace Spydersoft.Identity.Models.Identity
{
    public class ApplicationRolesViewModel
    {
        public IQueryable<ApplicationRole> Roles { get; set; }
    }
}