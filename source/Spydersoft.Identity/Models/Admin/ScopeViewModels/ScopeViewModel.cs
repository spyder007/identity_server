using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ScopeViewModels
{
    /// <summary>
    /// Class ScopeViewModel.
    /// Implements the <see cref="BaseAdminViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminViewModel" />
    public class ScopeViewModel : BaseAdminViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeViewModel"/> class.
        /// </summary>
        public ScopeViewModel()
        {
            NavBar = new ScopeNavBarViewModel(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeViewModel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public ScopeViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = id;
        }

        /// <summary>
        /// Gets or sets the nav bar.
        /// </summary>
        /// <value>The nav bar.</value>
        public ScopeNavBarViewModel NavBar { get; set; }


        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

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
        /// Gets or sets a value indicating whether this <see cref="ScopeViewModel"/> is emphasize.
        /// </summary>
        /// <value><c>true</c> if emphasize; otherwise, <c>false</c>.</value>
        [Display(Name = "Emphasize")]
        public bool Emphasize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScopeViewModel"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        [Display(Name = "Required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show in discovery document].
        /// </summary>
        /// <value><c>true</c> if [show in discovery document]; otherwise, <c>false</c>.</value>
        [Display(Name = "Show in Discovery Document")]
        public bool ShowInDiscoveryDocument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScopeViewModel"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
    }

}