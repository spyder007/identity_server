using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceScopesViewModel : BaseApiResourceCollectionViewModel<ApiResourceScopeViewModel>
    {
    }

    public class ApiResourceScopeViewModel : BaseAdminChildItemViewModel
    {
        public ApiResourceScopeViewModel()
        {
        }

        public string Scope { get; set; }
    }


}

