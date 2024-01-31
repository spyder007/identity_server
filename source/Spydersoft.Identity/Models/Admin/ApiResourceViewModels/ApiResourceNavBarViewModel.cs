namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceNavBarViewModel(ApiResourceViewModel parent) : BaseAdminNavBar<ApiResourceViewModel>(parent)
    {
        public override string Name => Parent.Name;
    }
}