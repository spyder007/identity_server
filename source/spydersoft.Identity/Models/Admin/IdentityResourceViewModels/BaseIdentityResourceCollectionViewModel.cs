namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class BaseIdentityResourceCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar GetNavBar()
        {
            return new IdentityResourceNavBarViewModel();
        }
    }
}
