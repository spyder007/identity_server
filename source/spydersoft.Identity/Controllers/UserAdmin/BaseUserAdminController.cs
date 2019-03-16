﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using spydersoft.Identity.Models;
using spydersoft.Identity.Models.Identity;

namespace spydersoft.Identity.Controllers.UserAdmin
{
    public class BaseUserAdminController : BaseController
    {
        public BaseUserAdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        protected UserManager<ApplicationUser> UserManager { get; }
        protected RoleManager<ApplicationRole> RoleManager { get; }
    }
}
