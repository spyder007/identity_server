namespace Spydersoft.Identity.Options
{
    /// <summary>
    /// Class ZipkinOptions.
    /// </summary>
    public class ZipkinOptions
    {
        /// <summary>
        /// The settings key
        /// </summary>
        public const string SettingsKey = "Zipkin";

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; } = "http://localhost:12345/zipkin";
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; } = "identity.server";
    }
}