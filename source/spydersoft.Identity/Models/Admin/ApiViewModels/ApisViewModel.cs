using System.Linq;

namespace spydersoft.Identity.Models.Admin.ApiViewModels
{
    public class ApisViewModel
    {
        public IQueryable<ApiViewModel> Apis { get; set; }
    }
}
