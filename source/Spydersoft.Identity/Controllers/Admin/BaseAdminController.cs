using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Authorization;

namespace Spydersoft.Identity.Controllers.Admin
{
    /// <summary>
    /// Class BaseAdminController.
    /// Implements the <see cref="BaseController" />
    /// </summary>
    /// <seealso cref="BaseController" />
    [Authorize(Roles = "admin")]
    public class BaseAdminController(ConfigurationDbContext context, IMapper mapper) : BaseController(mapper)
    {
        /// <summary>
        /// Gets or sets the configuration database context.
        /// </summary>
        /// <value>The configuration database context.</value>
        protected ConfigurationDbContext ConfigDbContext { get; set; } = context;
    }
}