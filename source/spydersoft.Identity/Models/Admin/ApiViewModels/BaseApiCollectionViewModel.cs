namespace spydersoft.Identity.Models.Admin.ApiViewModels
{
    public class BaseApiCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, ApiViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar<ApiViewModel> GetNavBar(ApiViewModel parent)
        {
            return new ApiNavBarViewModel(parent);
        }
    }
}
