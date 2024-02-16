namespace Spydersoft.Identity.Options
{
    /// <summary>
    /// Class ProviderOptions.
    /// </summary>
    public class ProviderOptions
    {
        /// <summary>
        /// The settings key
        /// </summary>
        public const string SettingsKey = "ProviderSettings";

        /// <summary>
        /// Gets or sets the google client identifier.
        /// </summary>
        /// <value>The google client identifier.</value>
        public string GoogleClientId { get; set; } = "id";
        /// <summary>
        /// Gets or sets the google client secret.
        /// </summary>
        /// <value>The google client secret.</value>
        public string GoogleClientSecret { get; set; } = "secret";
    }
}