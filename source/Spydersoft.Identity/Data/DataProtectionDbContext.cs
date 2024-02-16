using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Spydersoft.Identity.Data
{
    /// <summary>
    /// Class DataProtectionDbContext.
    /// Implements the <see cref="DbContext" />
    /// Implements the <see cref="IDataProtectionKeyContext" />
    /// </summary>
    /// <seealso cref="DbContext" />
    /// <seealso cref="IDataProtectionKeyContext" />
    public class DataProtectionDbContext(DbContextOptions<DataProtectionDbContext> options) : DbContext(options), IDataProtectionKeyContext
    {
        /// <summary>
        /// Gets or sets the data protection keys.
        /// </summary>
        /// <value>The data protection keys.</value>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}