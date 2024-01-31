using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using SendGrid;
using SendGrid.Helpers.Mail;

using Spydersoft.Identity.Options;

namespace Spydersoft.Identity.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender(IOptions<SendgridOptions> options) : IEmailSender
    {
        private readonly SendgridOptions _options = options.Value;

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.Run(() => SendEmail(email, subject, message));
        }


        private async Task SendEmail(string email, string subject, string message)
        {
            var client = new SendGridClient(_options.ApiKey);
            var from = new EmailAddress(_options.EmailFromAddress, _options.EmailFrom);
            var to = new EmailAddress(email);
            var htmlContent = message;
            var plainContent = HtmlToPlainText(message);
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainContent, htmlContent);
            msg.SetClickTracking(false, false);
            _ = await client.SendEmailAsync(msg);
        }

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