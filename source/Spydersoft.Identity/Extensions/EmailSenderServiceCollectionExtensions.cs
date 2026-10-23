using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spydersoft.Email.Client;
using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Services;

namespace Spydersoft.Identity.Extensions
{
    /// <summary>
    /// Registers the account email sender.
    /// </summary>
    public static class EmailSenderServiceCollectionExtensions
    {
        /// <summary>
        /// Sends account email through the email platform service when <c>EmailClient:BaseUrl</c> is
        /// configured; otherwise falls back to calling Resend directly.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAccountEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration["EmailClient:BaseUrl"]))
            {
                _ = services.AddSpydersoftEmailClient(configuration);
                _ = services.AddTransient<IEmailSender, EmailServiceSender>();
            }
            else
            {
                _ = services.AddHttpClient<IEmailSender, EmailSender>();
            }

            return services;
        }
    }
}
