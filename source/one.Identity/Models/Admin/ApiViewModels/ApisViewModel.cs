using System.Linq;

namespace one.Identity.Models.Admin.ApiViewModels
{
    public class ApisViewModel
    {
        public IQueryable<ApiViewModel> Apis { get; set; }
    }
}
