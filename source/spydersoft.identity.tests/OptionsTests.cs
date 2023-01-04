using spydersoft.Identity.Options;

namespace spydersoft.identity.tests
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
    }
}