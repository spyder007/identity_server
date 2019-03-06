using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class IdentityResourceNavBarViewModel
    {
        public int Id { get; set; }

        public string MainActive { get; private set; }
        public string ClaimsActive { get; private set; }
        public string PropertiesActive { get; private set; }

        public void SetActive(object model)
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
