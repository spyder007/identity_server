namespace spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceNavBarViewModel : BaseAdminNavBar<ApiResourceViewModel>
    {
        public ApiResourceNavBarViewModel(ApiResourceViewModel parent) : base(parent)
        {
        }

        public override string Name => Parent.Name;
    }
}
