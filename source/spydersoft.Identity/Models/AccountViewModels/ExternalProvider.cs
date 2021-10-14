using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Models.AccountViewModels
{
    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }

        public string LniCssClass
        {
            get
            {
                switch (DisplayName)
                {
                    case "Google":
                        return "fab fa-2x fa-google";

                    default:
                        return "fab fa-2x fa-openid";
                }
            }
        }

        public string ButtonCssClass
        {
            get
            {
                switch (DisplayName)
                {
                    case "Google":
                        return "danger-btn-outline";

                    default:
                        return "primary-btn-outline";
                }
            }
        }
    }
}
