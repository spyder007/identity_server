namespace Spydersoft.Identity.Options
{
    /// <summary>
    /// Options for sending transactional email through Resend (https://resend.com).
    /// </summary>
    public class ResendOptions
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public const string Name = "Resend";

        /// <summary>
        /// Gets or sets the Resend API key (e.g. <c>re_...</c>).
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the display name shown in the From header.
        /// </summary>
        /// <value>The from display name.</value>
        public string EmailFrom { get; set; }

        /// <summary>
        /// Gets or sets the From address. The domain must be a verified sender in Resend.
        /// </summary>
        /// <value>The from address.</value>
        public string EmailFromAddress { get; set; }
    }
}