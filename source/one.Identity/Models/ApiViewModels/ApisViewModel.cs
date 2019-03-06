using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ApiViewModels
{
    public class ApisViewModel
    {
        public IQueryable<ApiViewModel> Apis { get; set; }
    }
}
