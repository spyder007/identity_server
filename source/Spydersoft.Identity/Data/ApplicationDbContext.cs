using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Data
{
    /// <summary>
    /// Class ApplicationDbContext.
    /// Implements the <see cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{Models.Identity.ApplicationUser, Models.Identity.ApplicationRole, System.String}" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{Models.Identity.ApplicationUser, Models.Identity.ApplicationRole, System.String}" />
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
    }
}