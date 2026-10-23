using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Abstractions;

using Spydersoft.Email.Client;
using Spydersoft.Identity.Services;

namespace Spydersoft.identity.tests
{
    public class EmailServiceSenderTests
    {
        private sealed class RecordingEmailClient : IEmailClient
        {
            public List<EmailMessage> Sent { get; } = [];

            public Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
            {
                Sent.Add(message);
                return Task.FromResult(new EmailSendResult(Guid.NewGuid(), "re_fake", EmailStatus.Sent, DateTimeOffset.UtcNow));
            }

            public Task<EmailStatusView> GetStatusAsync(Guid id, CancellationToken cancellationToken = default) =>
                Task.FromResult<EmailStatusView>(null);

            public Task<IReadOnlyList<EmailEventView>> GetEventsAsync(Guid id, CancellationToken cancellationToken = default) =>
                Task.FromResult<IReadOnlyList<EmailEventView>>([]);

            public Task<EmailPage> SearchAsync(EmailQuery query, CancellationToken cancellationToken = default) =>
                Task.FromResult(new EmailPage([], 0, query.Skip, query.Limit));
        }

        [Test]
        public async Task SendEmailAsync_SendsOneTaggedHtmlAndTextMessage()
        {
            var client = new RecordingEmailClient();
            var sender = new EmailServiceSender(client, NullLogger<EmailServiceSender>.Instance);

            await sender.SendEmailAsync("user@example.com", "Confirm your email", "<p>Hello<br/>there</p>");

            var message = client.Sent[0];
            Assert.That(client.Sent, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(message.To, Is.EqualTo(new[] { "user@example.com" }));
                Assert.That(message.Subject, Is.EqualTo("Confirm your email"));
                Assert.That(message.Html, Is.EqualTo("<p>Hello<br/>there</p>"));
                Assert.That(message.Text, Does.Contain("Hello").And.Not.Contain("<p>"));
                Assert.That(message.Tags["app"], Is.EqualTo("identity"));
            });
        }
    }
}
