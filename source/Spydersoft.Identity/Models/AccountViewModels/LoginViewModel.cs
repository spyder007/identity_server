using System.Collections.Generic;
using System.Linq;

namespace Spydersoft.Identity.Models.AccountViewModels
{
    /// <summary>
    /// Class LoginViewModel.
    /// Implements the <see cref="LoginInputModel" />
    /// </summary>
    /// <seealso cref="LoginInputModel" />
    public class LoginViewModel : LoginInputModel
    {
        /// <summary>
        /// Gets or sets the allow remember login.
        /// </summary>
        /// <value>The allow remember login.</value>
        public bool AllowRememberLogin { get; set; } = true;
        /// <summary>
        /// Gets or sets the enable local login.
        /// </summary>
        /// <value>The enable local login.</value>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        /// Gets or sets the external providers.
        /// </summary>
        /// <value>The external providers.</value>
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = [];
        /// <summary>
        /// Gets the visible external providers.
        /// </summary>
        /// <value>The visible external providers.</value>
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        /// <summary>
        /// Gets the is external login only.
        /// </summary>
        /// <value>The is external login only.</value>
        public bool IsExternalLoginOnly => !EnableLocalLogin && ExternalProviders?.Count() == 1;
        /// <summary>
        /// Gets the external login scheme.
        /// </summary>
        /// <value>The external login scheme.</value>
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}