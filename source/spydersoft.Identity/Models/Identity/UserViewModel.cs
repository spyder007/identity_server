using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Models.Identity
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public string Password { get; set; }
        public bool IsNewUser { get; set; }

        public IQueryable<string> Roles { get; set; }
        public IQueryable<string> AvailableRoles { get; set; }
        public string SelectedAvailableRole { get; set; }
    }
}
