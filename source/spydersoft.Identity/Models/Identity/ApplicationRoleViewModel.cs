using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace spydersoft.Identity.Models.Identity
{
    public class ApplicationRoleViewModel
    {
        public ApplicationRole Role { get; set; }
        public IQueryable<Claim> Claims { get; set; }
        public ClaimModel NewClaim { get; set; }
    }

    public class ClaimModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
