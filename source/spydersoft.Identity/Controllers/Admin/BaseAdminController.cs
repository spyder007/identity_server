using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;

namespace spydersoft.Identity.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(ConfigurationDbContext context, MapperConfiguration mapperConfig) : base(mapperConfig)
        {
            ConfigDbContext = context;
        }

        protected ConfigurationDbContext ConfigDbContext { get; set; }
    }
}
