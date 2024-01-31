namespace Spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourceNavBarViewModel(IdentityResourceViewModel parent) : BaseAdminNavBar<IdentityResourceViewModel>(parent)
    {
        public override string Name => Parent.Name;
    }
}