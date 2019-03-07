using System.Linq;

namespace one.Identity.Models.Admin.ClientViewModels
{
    public class ClientsViewModel
    {
        public IQueryable<ClientViewModel> Clients { get; set; }
    }
}
