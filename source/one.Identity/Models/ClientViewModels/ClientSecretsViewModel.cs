using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class ClientSecretsViewModel : BaseClientCollectionViewModel<ClientSecretViewModel>
    {
    }

    public class ClientSecretViewModel : BaseAdminChildItemViewModel
    {
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name="Expiration")]
        public DateTime? Expiration { get; set; }

        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Type")]
        [DefaultValue("SharedSecret")]
        public string Type { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Description")]
        public string Value { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        [Display(Name = "Created")]
        public DateTime Created { get; set; }
    }
}
