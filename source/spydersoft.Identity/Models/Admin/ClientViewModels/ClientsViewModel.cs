using System.Linq;

namespace spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientsViewModel
    {
        public IQueryable<ClientViewModel> Clients { get; set; }
    }
}
