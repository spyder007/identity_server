using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class ScopeNavBarViewModel : BaseAdminNavBar<ScopeViewModel>
    {
        public ScopeNavBarViewModel(ScopeViewModel parent) : base(parent)
        {

        }

        public override string Name => Parent.Name;

    }
}
