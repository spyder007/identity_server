using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;

namespace one.Identity.Controllers.Admin.Client
{
    [Authorize(Roles = "admin")]
    public class BaseClientController : BaseController
    {
        public BaseClientController(ConfigurationDbContext context)
        {
            ConfigDbContext = context;
        }

        protected ConfigurationDbContext ConfigDbContext { get; set; }

    }
}
