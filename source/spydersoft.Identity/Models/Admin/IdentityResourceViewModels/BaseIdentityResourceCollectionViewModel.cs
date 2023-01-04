namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class BaseIdentityResourceCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, IdentityResourceViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar<IdentityResourceViewModel> GetNavBar(IdentityResourceViewModel parent)
        {
            return new IdentityResourceNavBarViewModel(parent);
        }
    }
}