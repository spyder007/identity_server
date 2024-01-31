namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class ScopeNavBarViewModel(ScopeViewModel parent) : BaseAdminNavBar<ScopeViewModel>(parent)
    {
        public override string Name => Parent.Name;

    }
}