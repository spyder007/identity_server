namespace one.Identity.Models.Admin.ApiViewModels
{
    public class BaseApiCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar GetNavBar()
        {
            return new ApiNavBarViewModel();
        }
    }
}
