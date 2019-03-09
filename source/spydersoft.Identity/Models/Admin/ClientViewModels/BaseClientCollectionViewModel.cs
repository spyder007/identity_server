namespace spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class BaseClientCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar GetNavBar()
        {
            return new NavBarViewModel();
        }
    }
}
