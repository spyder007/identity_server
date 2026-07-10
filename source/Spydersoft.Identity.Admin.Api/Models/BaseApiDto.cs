namespace Spydersoft.Identity.Admin.Api.Models
{
    /// <summary>
    /// Base class for all API DTOs with an integer identifier.
    /// </summary>
    public abstract class BaseApiDto
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
    }
}
