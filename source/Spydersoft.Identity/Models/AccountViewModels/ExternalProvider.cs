namespace Spydersoft.Identity.Models.AccountViewModels
{
    /// <summary>
    /// Class ExternalProvider.
    /// </summary>
    public class ExternalProvider
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the authentication scheme.
        /// </summary>
        /// <value>The authentication scheme.</value>
        public string AuthenticationScheme { get; set; }

        /// <summary>
        /// Gets the lni CSS class.
        /// </summary>
        /// <value>The lni CSS class.</value>
        public string LniCssClass => DisplayName switch
        {
            "Google" => "fab fa-2x fa-google",
            _ => "fab fa-2x fa-openid",
        };

        /// <summary>
        /// Gets the button CSS class.
        /// </summary>
        /// <value>The button CSS class.</value>
        public string ButtonCssClass => DisplayName switch
        {
            "Google" => "danger-btn-outline",
            _ => "primary-btn-outline",
        };
    }
}