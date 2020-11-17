namespace spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class BaseApiResourceCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, ApiResourceViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar<ApiResourceViewModel> GetNavBar(ApiResourceViewModel parent)
        {
            return new ApiResourceNavBarViewModel(parent);
        }
    }
}
