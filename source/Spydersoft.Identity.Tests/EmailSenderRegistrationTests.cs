using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spydersoft.Email.Client;
using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Services;

namespace Spydersoft.identity.tests
{
    public class EmailSenderRegistrationTests
    {
        private static ServiceProvider Build(Dictionary<string, string?> settings)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountEmailSender(configuration);
            return services.BuildServiceProvider();
        }

        [Test]
        public void WithEmailClientBaseUrl_UsesEmailService()
        {
            using var provider = Build(new Dictionary<string, string?> { ["EmailClient:BaseUrl"] = "http://email.test" });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(provider.GetRequiredService<IEmailSender>(), Is.TypeOf<EmailServiceSender>());
                Assert.That(provider.GetService<IEmailClient>(), Is.Not.Null);
            }
        }

        [Test]
        public void WithoutEmailClientBaseUrl_FallsBackToDirectResend()
        {
            using var provider = Build([]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(provider.GetRequiredService<IEmailSender>(), Is.TypeOf<EmailSender>());
                Assert.That(provider.GetServices<IEmailClient>().Any(), Is.False);
            }
        }
    }
}
