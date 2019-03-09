namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourceNavBarViewModel : BaseAdminNavBar<IdentityResourceViewModel>
    {
        public IdentityResourceNavBarViewModel(IdentityResourceViewModel parent) : base(parent)
        {

        }

        public override string Name => Parent.Name;
    }
}
