using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class BaseClientChildItemViewModel : BaseViewModel, IClientCollectionViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
    }
}
