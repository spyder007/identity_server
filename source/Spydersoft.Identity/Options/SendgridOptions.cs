namespace Spydersoft.Identity.Options
{
    /// <summary>
    /// Class SendgridOptions.
    /// </summary>
    public class SendgridOptions
    {
        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "Sendgrid";
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey { get; set; }
        /// <summary>
        /// Gets or sets the email from.
        /// </summary>
        /// <value>The email from.</value>
        public string EmailFrom { get; set; }
        /// <summary>
        /// Gets or sets the email from address.
        /// </summary>
        /// <value>The email from address.</value>
        public string EmailFromAddress { get; set; }
    }
}