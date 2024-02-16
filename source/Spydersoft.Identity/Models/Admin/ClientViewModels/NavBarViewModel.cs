namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class NavBarViewModel.
    /// Implements the <see cref="BaseAdminNavBar{TMainViewModel}" />
    /// </summary>
    /// <seealso cref="BaseAdminNavBar{TMainViewModel}" />
    public class NavBarViewModel(ClientViewModel parent) : BaseAdminNavBar<ClientViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.ClientName;

    }
}