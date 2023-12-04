namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class ScopeNavBarViewModel : BaseAdminNavBar<ScopeViewModel>
    {
        public ScopeNavBarViewModel(ScopeViewModel parent) : base(parent)
        {

        }

        public override string Name => Parent.Name;

    }
}