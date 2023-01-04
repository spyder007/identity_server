using System.Linq;

namespace spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourcesViewModel
    {
        public IQueryable<ApiResourceViewModel> Apis { get; set; }
    }
}