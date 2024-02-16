using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourceSecretsViewModel.
    /// Implements the <see cref="BaseApiResourceCollectionViewModel{T}" />
    /// </summary>
    /// <seealso cref="BaseApiResourceCollectionViewModel{T}" />
    public class ApiResourceSecretsViewModel : BaseApiResourceCollectionViewModel<ApiResourceSecretViewModel>
    {
    }

    /// <summary>
    /// Class ApiResourceSecretViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ApiResourceSecretViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        [DataType(DataType.DateTime)]
        [Display(Name = "Expiration")]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Type")]
        [DefaultValue("SharedSecret")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DataType(DataType.DateTime)]
        [Required]
        [Display(Name = "Created")]
        public DateTime Created { get; set; }
    }
}