using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ApiViewModels
{
    public class ApiViewModel
    {

        public ApiViewModel()
        {
            NavBar = new ApiNavBarViewModel();
            NavBar.SetActive(this);
        }

        public ApiViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = Id;
        }

        public ApiNavBarViewModel NavBar { get; set; }

        public int Id { get; set; }

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
