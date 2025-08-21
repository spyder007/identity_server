using System.Reflection.Emit;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Data
{
    /// <summary>
    /// Class ApplicationDbContext.
    /// Implements the <see cref="IdentityDbContext{TUser, TRole, TKey}" />
    /// </summary>
    /// <seealso cref="IdentityDbContext{TUser, TRole, TKey}" />
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
    }
}