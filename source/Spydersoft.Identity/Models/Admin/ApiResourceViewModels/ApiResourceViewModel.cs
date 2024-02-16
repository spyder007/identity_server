using System;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourceViewModel.
    /// Implements the <see cref="BaseAdminViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminViewModel" />
    public class ApiResourceViewModel : BaseAdminViewModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceViewModel"/> class.
        /// </summary>
        public ApiResourceViewModel()
        {
            NavBar = new ApiResourceNavBarViewModel(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceViewModel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public ApiResourceViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = Id;
        }

        /// <summary>
        /// Gets or sets the nav bar.
        /// </summary>
        /// <value>The nav bar.</value>
        public ApiResourceNavBarViewModel NavBar { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ApiResourceViewModel"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [non editable].
        /// </summary>
        /// <value><c>true</c> if [non editable]; otherwise, <c>false</c>.</value>
        [Display(Name = "Non Editable")]
        public bool NonEditable { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the last accessed.
        /// </summary>
        /// <value>The last accessed.</value>
        public DateTime LastAccessed { get; set; }
        /// <summary>
        /// Gets or sets the updated.
        /// </summary>
        /// <value>The updated.</value>
        public DateTime Updated { get; set; }
    }
}