using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Spydersoft.Identity.Services;

namespace Spydersoft.Identity.Extensions
{
    /// <summary>
    /// Class EmailSenderExtensions.
    /// </summary>
    public static class EmailSenderExtensions
    {
        /// <summary>
        /// Sends the email confirmation asynchronous.
        /// </summary>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="email">The email.</param>
        /// <param name="link">The link.</param>
        /// <returns>Task.</returns>
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}