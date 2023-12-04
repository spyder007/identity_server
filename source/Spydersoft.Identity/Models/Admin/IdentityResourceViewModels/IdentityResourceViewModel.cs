using System;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.IdentityResourceViewModels
{
    public class IdentityResourceViewModel : BaseAdminViewModel
    {
        public IdentityResourceViewModel()
        {
            NavBar = new IdentityResourceNavBarViewModel(this);
        }

        public IdentityResourceViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = Id;
        }

        public IdentityResourceNavBarViewModel NavBar { get; set; }

        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Emphasize")]
        public bool Emphasize { get; set; }

        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Required")]
        public bool Required { get; set; }

        [Display(Name = "Show in Discovery Document")]
        public bool ShowInDiscoveryDocument { get; set; }

        public DateTime Created { get; set; }

        [Display(Name = "Non Editable")]
        public bool NonEditable { get; set; }

        public DateTime? Updated { get; set; }
    }
}