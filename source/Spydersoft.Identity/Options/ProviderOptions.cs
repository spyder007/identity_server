namespace Spydersoft.Identity.Options
{
    public class ProviderOptions
    {
        public const string SettingsKey = "ProviderSettings";

        public string GoogleClientId { get; set; } = "id";
        public string GoogleClientSecret { get; set; } = "secret";
    }
}