using System;
using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceViewModel : BaseAdminViewModel
    { 

        public ApiResourceViewModel()
        {
            NavBar = new ApiResourceNavBarViewModel(this);
        }

        public ApiResourceViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = Id;
        }

        public ApiResourceNavBarViewModel NavBar { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name ="Enabled")]
        public bool Enabled { get; set; }

        [Display(Name="Non Editable")]
        public bool NonEditable { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime Updated { get; set; }
    }
}
