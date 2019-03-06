using System.Linq;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientsViewModel
    {
        public IQueryable<ClientViewModel> Clients { get; set; }
    }
}
