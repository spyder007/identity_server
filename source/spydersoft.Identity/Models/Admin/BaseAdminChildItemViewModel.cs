using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Models
{
    public class BaseAdminChildItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
    }
}
