namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class NavBarViewModel : BaseAdminNavBar<ClientViewModel>
    {
        public NavBarViewModel(ClientViewModel parent) : base(parent)
        {

        }

        public override string Name => Parent.ClientName;

    }
}