namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class BaseClientCollectionViewModel.
    /// Implements the <see cref="BaseAdminChildCollectionViewModel{TChildViewModel, TParentItemViewModel}" />
    /// </summary>
    /// <typeparam name="TChildViewModel">The type of the t child view model.</typeparam>
    /// <seealso cref="BaseAdminChildCollectionViewModel{TChildViewModel, TParentItemViewModel}" />
    public class BaseClientCollectionViewModel<TChildViewModel> : BaseAdminChildCollectionViewModel<TChildViewModel, ClientViewModel>
        where TChildViewModel : BaseAdminChildItemViewModel, new()
    {
        /// <summary>
        /// Gets the nav bar.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>BaseAdminNavBar&lt;ClientViewModel&gt;.</returns>
        public override BaseAdminNavBar<ClientViewModel> GetNavBar(ClientViewModel parent)
        {
            return new NavBarViewModel(parent);
        }
    }
}