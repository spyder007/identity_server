using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class BaseIdentityResourceCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar GetNavBar()
        {
            return new IdentityResourceNavBarViewModel();
        }
    }
}
