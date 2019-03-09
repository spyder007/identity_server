﻿namespace spydersoft.Identity.Models.Admin.ApiViewModels
{
    public class ApiNavBarViewModel : BaseAdminNavBar
    {
        public string MainActive { get; private set; }
        public string ScopesActive { get; private set; }
        public string PropertiesActive { get; private set; }
        public string ClaimsActive { get; private set; }
        public string SecretsActive { get; private set; }

        public override void SetActive(object model)
        {
            
        }
    }
}