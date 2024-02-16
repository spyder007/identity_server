using System.Linq;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientsViewModel.
    /// </summary>
    public class ClientsViewModel
    {
        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>The clients.</value>
        public IQueryable<ClientViewModel> Clients { get; set; }
    }
}