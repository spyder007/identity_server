namespace Spydersoft.Identity.Models.Admin
{
    /// <summary>
    /// Class BaseAdminChildItemViewModel.
    /// Implements the <see cref="BaseAdminViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminViewModel" />
    public class BaseAdminChildItemViewModel : BaseAdminViewModel
    {
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        public int ParentId { get; set; }
    }
}