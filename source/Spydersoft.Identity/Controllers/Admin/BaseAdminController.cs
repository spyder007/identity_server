using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Authorization;

namespace Spydersoft.Identity.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class BaseAdminController(ConfigurationDbContext context, IMapper mapper) : BaseController(mapper)
    {
        protected ConfigurationDbContext ConfigDbContext { get; set; } = context;
    }
}