namespace one.Identity.Models.ClientViewModels
{
    public class NavBarViewModel
    {
        public int Id { get; set; }

        public string MainActive { get; private set; }
        public string ScopesActive { get; private set; }
        public string RedirectActive { get; private set; }
        public string ClaimsActive { get; private set; }
        public string CorsOriginsActive { get; private set; }
        public string GrantTypesActive { get; private set; }
        public string IdpRestrictionsActive { get; private set; }
        public string PostLogoutActive { get; private set; }
        public string PropertiesActive { get; private set; }
        public string ClientSecretActive { get; private set; }

        public void SetActive(object model)
        {
            MainActive = string.Empty;
            ScopesActive = string.Empty;

            if (model is ClientViewModel)
            {
                MainActive = "active";
            }

            if (model is ClientScopesViewModel)
            {
                ScopesActive = "active";
            }

            if (model is ClientRedirectsViewModel)
            {
                RedirectActive = "active";
            }

            if (model is ClientClaimsViewModel)
            {
                ClaimsActive = "active";
            }

            if (model is ClientCorsOriginsViewModel)
            {
                CorsOriginsActive = "active";
            }

            if (model is ClientGrantTypesViewModel)
            {
                GrantTypesActive = "active";
            }

            if (model is ClientIdpRestrictionsViewModel)
            {
                IdpRestrictionsActive = "active";
            }

            if (model is ClientPostLogoutRedirectUrisViewModel)
            {
                PostLogoutActive = "active";
            }

            if (model is ClientPropertiesViewModel)
            {
                PropertiesActive = "active";
            }

            if (model is ClientSecretsViewModel)
            {
                ClientSecretActive = "active";
            }
        }

    }
}
