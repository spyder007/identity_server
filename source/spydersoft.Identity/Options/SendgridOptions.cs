namespace Spydersoft.Identity.Options
{
    public class SendgridOptions
    {
        public const string Name = "Sendgrid";
        public string ApiKey { get; set; }
        public string EmailFrom { get; set; }
        public string EmailFromAddress { get; set; }
    }
}