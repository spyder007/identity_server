using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Options;

namespace Spydersoft.Identity.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // It posts to the Resend transactional email API (https://resend.com).
    /// <summary>
    /// Class EmailSender.
    /// Implements the <see cref="IEmailSender" />
    /// </summary>
    /// <seealso cref="IEmailSender" />
    public class EmailSender(HttpClient httpClient, IOptions<ResendOptions> options, ILogger<EmailSender> logger) : IEmailSender
    {
        /// <summary>The Resend "send email" endpoint.</summary>
        private const string SendEndpoint = "https://api.resend.com/emails";

        /// <summary>
        /// The options
        /// </summary>
        private readonly ResendOptions _options = options.Value;

        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var payload = new
            {
                from = $"{_options.EmailFrom} <{_options.EmailFromAddress}>",
                to = new[] { email },
                subject,
                html = message,
                text = HtmlToPlainText(message)
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, SendEndpoint)
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to send email to {Email} via Resend: {StatusCode} {Body}",
                    email, (int)response.StatusCode, body);
                throw new InvalidOperationException(
                    $"Failed to send email via Resend (HTTP {(int)response.StatusCode}).");
            }

            logger.LogInformation("Sent email to {Email} via Resend.", email);
        }

        /// <summary>
        /// HTMLs to plain text.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>System.String.</returns>
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
    }
}