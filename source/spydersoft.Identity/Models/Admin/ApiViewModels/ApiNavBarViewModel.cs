namespace spydersoft.Identity.Models.Admin.ApiViewModels
{
    public class ApiNavBarViewModel : BaseAdminNavBar<ApiViewModel>
    {
        public ApiNavBarViewModel(ApiViewModel parent) : base(parent)
        {
        }

        public override string Name => Parent.Name;
    }
}
