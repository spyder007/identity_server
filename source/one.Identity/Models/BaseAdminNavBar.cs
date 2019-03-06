using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models
{
    public abstract class BaseAdminNavBar
    {
        public int Id { get; set; }

        public abstract void SetActive(object model);
    }
}
