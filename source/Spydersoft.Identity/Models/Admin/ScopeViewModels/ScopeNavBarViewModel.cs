namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    /// <summary>
    /// Class ScopeNavBarViewModel.
    /// Implements the <see cref="Admin.BaseAdminNavBar{ScopeViewModels.ScopeViewModel}" />
    /// </summary>
    /// <seealso cref="Admin.BaseAdminNavBar{ScopeViewModels.ScopeViewModel}" />
    public class ScopeNavBarViewModel(ScopeViewModel parent) : BaseAdminNavBar<ScopeViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.Name;

    }
}