using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Models.Admin.ScopeViewModels
{
    public class ScopeViewModel : BaseAdminViewModel
    {
        public ScopeViewModel()
        {
            NavBar = new ScopeNavBarViewModel(this);
        }

        public ScopeViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = id;
        }

        public ScopeNavBarViewModel NavBar { get; set; }


        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Emphasize")]
        public bool Emphasize { get; set; }

        [Display(Name = "Required")]
        public bool Required { get; set; }

        [Display(Name = "Show in Discovery Document")]
        public bool ShowInDiscoveryDocument { get; set; }
    }

}
