namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    /// <summary>
    /// Class ScopeNavBarViewModel.
    /// Implements the <see cref="BaseAdminNavBar{TMainViewModel}" />
    /// </summary>
    /// <seealso cref="BaseAdminNavBar{TMainViewModel}" />
    public class ScopeNavBarViewModel(ScopeViewModel parent) : BaseAdminNavBar<ScopeViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.Name;

    }
}