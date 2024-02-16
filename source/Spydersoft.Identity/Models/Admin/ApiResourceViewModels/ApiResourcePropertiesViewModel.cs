using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourcePropertiesViewModel.
    /// Implements the <see cref="BaseApiResourceCollectionViewModel{T}" />
    /// </summary>
    /// <seealso cref="BaseApiResourceCollectionViewModel{T}" />
    public class ApiResourcePropertiesViewModel : BaseApiResourceCollectionViewModel<ApiResourcePropertyViewModel>
    {
    }

    /// <summary>
    /// Class ApiResourcePropertyViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ApiResourcePropertyViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}