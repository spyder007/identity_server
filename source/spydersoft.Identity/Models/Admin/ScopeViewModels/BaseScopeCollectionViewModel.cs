namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class BaseScopeCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T, ScopeViewModel> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar<ScopeViewModel> GetNavBar(ScopeViewModel parent)
        {
            return new ScopeNavBarViewModel(parent);
        }
    }
}