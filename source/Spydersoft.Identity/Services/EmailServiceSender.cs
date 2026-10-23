using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Spydersoft.Email.Client;
using Spydersoft.Identity.Core.Services;

namespace Spydersoft.Identity.Services
{
    /// <summary>
    /// Sends account email (confirmation, password reset) through the Spydersoft email platform service.
    /// Implements the <see cref="IEmailSender" />
    /// </summary>
    /// <seealso cref="IEmailSender" />
    public class EmailServiceSender(IEmailClient emailClient, ILogger<EmailServiceSender> logger) : IEmailSender
    {
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var result = await emailClient.SendAsync(new EmailMessage
            {
                To = [email],
                Subject = subject,
                Html = message,
                Text = EmailSender.HtmlToPlainText(message),
                Tags = new Dictionary<string, string> { ["app"] = "identity" },
            });

            logger.LogInformation("Sent email {EmailId} through the email service.", result.Id);
        }
    }
}
