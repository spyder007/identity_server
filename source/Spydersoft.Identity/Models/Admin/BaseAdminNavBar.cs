namespace Spydersoft.Identity.Models.Admin
{
    /// <summary>
    /// Class BaseAdminNavBar.
    /// </summary>
    /// <typeparam name="TMainViewModel">The type of the t main view model.</typeparam>
    public abstract class BaseAdminNavBar<TMainViewModel>(TMainViewModel parent) where TMainViewModel : BaseAdminViewModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        protected TMainViewModel Parent { get; } = parent;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get; }
    }
}