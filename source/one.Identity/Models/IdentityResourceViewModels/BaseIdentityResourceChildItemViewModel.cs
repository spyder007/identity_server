using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class BaseIdentityResourceChildItemViewModel : BaseViewModel, IIdentityResourceCollectionViewModel
    {
        public int Id { get; set; }
        public int IdentityResourceId { get; set; }
    }
}
