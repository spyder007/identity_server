namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class NavBarViewModel(ClientViewModel parent) : BaseAdminNavBar<ClientViewModel>(parent)
    {
        public override string Name => Parent.ClientName;

    }
}