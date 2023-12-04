namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class BaseClientCollectionViewModel<TChildViewModel> : BaseAdminChildCollectionViewModel<TChildViewModel, ClientViewModel>
        where TChildViewModel : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar<ClientViewModel> GetNavBar(ClientViewModel parent)
        {
            return new NavBarViewModel(parent);
        }
    }
}