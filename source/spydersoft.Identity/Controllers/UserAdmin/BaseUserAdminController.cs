using AutoMapper;

using Microsoft.AspNetCore.Identity;

using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Controllers.UserAdmin
{
    public class BaseUserAdminController : BaseController
    {
        public BaseUserAdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper) : base(mapper)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        protected UserManager<ApplicationUser> UserManager { get; }
        protected RoleManager<ApplicationRole> RoleManager { get; }
    }
}