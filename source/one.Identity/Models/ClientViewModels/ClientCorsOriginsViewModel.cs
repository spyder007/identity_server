using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientCorsOriginsViewModel : BaseClientCollectionViewModel<ClientCorsOriginViewModel>
    {
    }

    public class ClientCorsOriginViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Origin")]
        public string Origin { get; set; }
    }
}
