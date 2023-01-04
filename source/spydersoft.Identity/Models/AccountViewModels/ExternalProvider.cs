namespace spydersoft.Identity.Models.AccountViewModels
{
    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }

        public string LniCssClass => DisplayName switch
        {
            "Google" => "fab fa-2x fa-google",
            _ => "fab fa-2x fa-openid",
        };

        public string ButtonCssClass => DisplayName switch
        {
            "Google" => "danger-btn-outline",
            _ => "primary-btn-outline",
        };
    }
}