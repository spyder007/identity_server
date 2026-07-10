namespace Spydersoft.Identity.Core.Models.Admin.IdentityResourceViewModels
{
    /// <summary>
    /// Class IdentityResourceNavBarViewModel.
    /// Implements the <see cref="BaseAdminNavBar{TMainViewModel}" />
    /// </summary>
    /// <seealso cref="BaseAdminNavBar{TMainViewModel}" />
    public class IdentityResourceNavBarViewModel(IdentityResourceViewModel parent) : BaseAdminNavBar<IdentityResourceViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.Name;
    }
}