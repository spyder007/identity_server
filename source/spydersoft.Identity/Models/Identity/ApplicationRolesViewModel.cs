using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Models.Identity
{
    public class ApplicationRolesViewModel
    {
        public IQueryable<ApplicationRole> Roles { get; set; }
    }
}
