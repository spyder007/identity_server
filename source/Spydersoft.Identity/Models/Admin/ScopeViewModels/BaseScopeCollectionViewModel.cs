namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    /// <summary>
    /// Class BaseScopeCollectionViewModel.
    /// Implements the <see cref="Admin.BaseAdminChildCollectionViewModel{T, ScopeViewModels.ScopeViewModel}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Admin.BaseAdminChildCollectionViewModel{T, ScopeViewModels.ScopeViewModel}" />
    public class BaseScopeCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, ScopeViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        /// <summary>
        /// Gets the nav bar.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>BaseAdminNavBar&lt;ScopeViewModel&gt;.</returns>
        public override BaseAdminNavBar<ScopeViewModel> GetNavBar(ScopeViewModel parent)
        {
            return new ScopeNavBarViewModel(parent);
        }
    }
}