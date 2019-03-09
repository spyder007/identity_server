using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;

namespace spydersoft.Identity.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(ConfigurationDbContext context)
        {
            ConfigDbContext = context;
        }

        protected ConfigurationDbContext ConfigDbContext { get; set; }
    }
}
