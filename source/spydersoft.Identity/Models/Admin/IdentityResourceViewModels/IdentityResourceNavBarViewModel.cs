namespace spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourceNavBarViewModel : BaseAdminNavBar
    {
        public string MainActive { get; private set; }
        public string ClaimsActive { get; private set; }
        public string PropertiesActive { get; private set; }

        public override void SetActive(object model)
        {
            if (model is IdentityResourceViewModel)
            {
                MainActive = "active";
            }

            if (model is IdentityResourceClaimsViewModel)
            {
                ClaimsActive = "active";
            }

            if (model is IdentityResourcePropertiesViewModel)
            {
                PropertiesActive = "active";
            }
        }
    }
}
