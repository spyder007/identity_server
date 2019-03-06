using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class BaseClientCollectionViewModel<T> : BaseAdminChildCollectionViewModel<T> where T : BaseAdminChildItemViewModel, new()
    {
        public override BaseAdminNavBar GetNavBar()
        {
            return new NavBarViewModel();
        }
    }
}
