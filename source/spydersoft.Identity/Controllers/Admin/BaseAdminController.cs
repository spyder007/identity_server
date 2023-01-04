using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Authorization;

namespace spydersoft.Identity.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(ConfigurationDbContext context, IMapper mapper) : base(mapper)
        {
            ConfigDbContext = context;
        }

        protected ConfigurationDbContext ConfigDbContext { get; set; }
    }
}