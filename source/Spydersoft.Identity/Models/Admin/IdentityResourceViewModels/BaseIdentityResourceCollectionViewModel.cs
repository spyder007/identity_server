namespace Spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    /// <summary>
    /// Class BaseIdentityResourceCollectionViewModel.
    /// Implements the <see cref="Admin.BaseAdminChildCollectionViewModel{T, IdentityResourceViewModels.IdentityResourceViewModel}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Admin.BaseAdminChildCollectionViewModel{T, IdentityResourceViewModels.IdentityResourceViewModel}" />
    public class BaseIdentityResourceCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, IdentityResourceViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        /// <summary>
        /// Gets the nav bar.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>BaseAdminNavBar&lt;IdentityResourceViewModel&gt;.</returns>
        public override BaseAdminNavBar<IdentityResourceViewModel> GetNavBar(IdentityResourceViewModel parent)
        {
            return new IdentityResourceNavBarViewModel(parent);
        }
    }
}