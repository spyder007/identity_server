using AutoMapper;

using Microsoft.AspNetCore.Identity;

using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Controllers.UserAdmin
{
    /// <summary>
    /// Class BaseUserAdminController.
    /// Implements the <see cref="BaseController" />
    /// </summary>
    /// <seealso cref="BaseController" />
    public class BaseUserAdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper) : BaseController(mapper)
    {
        /// <summary>
        /// Gets the user manager.
        /// </summary>
        /// <value>The user manager.</value>
        protected UserManager<ApplicationUser> UserManager { get; } = userManager;
        /// <summary>
        /// Gets the role manager.
        /// </summary>
        /// <value>The role manager.</value>
        protected RoleManager<ApplicationRole> RoleManager { get; } = roleManager;
    }
}