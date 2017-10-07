using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Admin.Client
{
    public class ClientsViewModel
    {
        public IQueryable<ClientViewModel> Clients { get; set; }
    }
}
