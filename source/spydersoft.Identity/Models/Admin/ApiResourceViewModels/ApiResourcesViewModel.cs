using System.Linq;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourcesViewModel
    {
        public IQueryable<ApiResourceViewModel> Apis { get; set; }
    }
}