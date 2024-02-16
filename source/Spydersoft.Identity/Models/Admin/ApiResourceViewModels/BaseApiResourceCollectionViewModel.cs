namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class BaseApiResourceCollectionViewModel.
    /// Implements the <see cref="BaseAdminChildCollectionViewModel{TChildItemViewModel, TParentItemViewModel}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="BaseAdminChildCollectionViewModel{TChildItemViewModel, TParentItemViewModel}" />
    public class BaseApiResourceCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, ApiResourceViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        /// <summary>
        /// Gets the nav bar.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>BaseAdminNavBar&lt;ApiResourceViewModel&gt;.</returns>
        public override BaseAdminNavBar<ApiResourceViewModel> GetNavBar(ApiResourceViewModel parent)
        {
            return new ApiResourceNavBarViewModel(parent);
        }
    }
}