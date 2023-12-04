using System.Linq;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientsViewModel
    {
        public IQueryable<ClientViewModel> Clients { get; set; }
    }
}