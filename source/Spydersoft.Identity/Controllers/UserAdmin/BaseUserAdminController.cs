using AutoMapper;

using Microsoft.AspNetCore.Identity;

using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Controllers.UserAdmin
{
    public class BaseUserAdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper) : BaseController(mapper)
    {
        protected UserManager<ApplicationUser> UserManager { get; } = userManager;
        protected RoleManager<ApplicationRole> RoleManager { get; } = roleManager;
    }
}