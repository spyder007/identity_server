namespace Spydersoft.Identity.Options
{
    public class ZipkinOptions
    {
        public const string SettingsKey = "Zipkin";

        public string Host { get; set; } = "http://localhost:12345/zipkin";
        public string ServiceName { get; set; } = "identity.server";
    }
}