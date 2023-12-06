using Spydersoft.Identity.Options;

namespace Spydersoft.identity.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SendgridOptionsPropertyTest()
        {
            var options = new SendgridOptions
            {
                ApiKey = "test",
                EmailFrom = "test name",
                EmailFromAddress = "test@email.com"
            };
            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Not.Null);
                Assert.That(options, Has.Property("ApiKey").TypeOf<string>());
                Assert.That(options, Has.Property("EmailFrom").TypeOf<string>());
                Assert.That(options, Has.Property("EmailFromAddress").TypeOf<string>());
            });
        }

        [Test]
        public void ConsentOptionsPropertyTest()
        {
            // All properties in ConsentOptions should have a default value, if they do not, this test will fail.
            var options = new ConsentOptions();
            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Not.Null);
                Assert.That(options, Has.Property("EnableOfflineAccess").TypeOf<bool>());
                Assert.That(options, Has.Property("OfflineAccessDisplayName").TypeOf<string>());
                Assert.That(options, Has.Property("OfflineAccessDescription").TypeOf<string>());
                Assert.That(options, Has.Property("MustChooseOneErrorMessage").TypeOf<string>());
                Assert.That(options, Has.Property("InvalidSelectionErrorMessage").TypeOf<string>());
            });
        }
    }
}