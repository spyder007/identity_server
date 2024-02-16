namespace Spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    /// <summary>
    /// Class IdentityResourceNavBarViewModel.
    /// Implements the <see cref="Admin.BaseAdminNavBar{IdentityResourceViewModels.IdentityResourceViewModel}" />
    /// </summary>
    /// <seealso cref="Admin.BaseAdminNavBar{IdentityResourceViewModels.IdentityResourceViewModel}" />
    public class IdentityResourceNavBarViewModel(IdentityResourceViewModel parent) : BaseAdminNavBar<IdentityResourceViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.Name;
    }
}