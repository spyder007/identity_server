using System.Linq;

namespace spydersoft.Identity.Models.Identity
{
    public class ApplicationRolesViewModel
    {
        public IQueryable<ApplicationRole> Roles { get; set; }
    }
}