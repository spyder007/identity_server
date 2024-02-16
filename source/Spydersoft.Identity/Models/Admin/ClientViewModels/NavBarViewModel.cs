namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class NavBarViewModel.
    /// Implements the <see cref="Admin.BaseAdminNavBar{ClientViewModels.ClientViewModel}" />
    /// </summary>
    /// <seealso cref="Admin.BaseAdminNavBar{ClientViewModels.ClientViewModel}" />
    public class NavBarViewModel(ClientViewModel parent) : BaseAdminNavBar<ClientViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.ClientName;

    }
}