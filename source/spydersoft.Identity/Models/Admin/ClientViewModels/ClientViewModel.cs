using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientViewModel : BaseAdminViewModel
    {

        public ClientViewModel()
        {
            NavBar = new NavBarViewModel(this);
        }

        public ClientViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = Id;
        }

        public NavBarViewModel NavBar { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Client URI")]
        public string ClientUri { get; set; }

        [Required]
        [Display(Name="Absolute Refresh Token Lifetime (seconds)")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [DefaultValue(2592000)]
        public int AbsoluteRefreshTokenLifetime { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Access Token Lifetime (seconds)")]
        [DefaultValue(3600)]
        public int AccessTokenLifetime { get; set; }

        [Display(Name = "Allow access tokens via browser")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Display(Name = "Allow offline access")]
        public bool AllowOfflineAccess { get; set; }

        [Display(Name = "Allow plain text Pkce")]
        public bool AllowPlainTextPkce { get; set; }

        [Display(Name = "Allow remember consent")]
        public bool AllowRememberConsent { get; set; }

        [Display(Name = "Always send client claims")]
        public bool AlwaysSendClientClaims { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Authorization Code Lifetime (seconds)")]
        [DefaultValue(300)]
        public int AuthorizationCodeLifetime { get; set; }

        [Display(Name = "Enable local login")]
        public bool EnableLocalLogin { get; set; }

        [Display(Name = "Is Enabled")]
        public bool Enabled { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Identity Token Lifetime (seconds)")]
        [DefaultValue(300)]
        public int IdentityTokenLifetime { get; set; }

        [Display(Name = "Include JWT Id")]
        public bool IncludeJwtId { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Logo URI")]
        public string LogoUri { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Protocol Type")]
        [DefaultValue("oidc")]
        public string ProtocolType { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Refresh Token Expiration (seconds)")]
        public int RefreshTokenExpiration { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Refresh Token Usage")]
        // TODO: Should be 1 or 0, based on Duende.IdentityServer.Models.TokenUsage.. figure out how to handle enums
        public int RefreshTokenUsage { get; set; }

        [Display(Name = "Require client secret")]
        public bool RequireClientSecret { get; set; }

        [Display(Name = "Require consent")]
        public bool RequireConsent { get; set; }

        [Display(Name = "Require PKCE")]
        public bool RequirePkce { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Sliding Refresh Token Lifetime (seconds)")]
        [DefaultValue(1296000)]
        public int SlidingRefreshTokenLifetime { get; set; }

        [Display(Name = "Update access token claims on refresh")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Display(Name = "Always include user claims in ID Token")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Display(Name = "Backchannel Logout session required")]
        public bool BackChannelLogoutSessionRequired { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Backchannel Logout URI")]
        public string BackChannelLogoutUri { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Client claims prefix")]
        public string ClientClaimsPrefix { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Consent Lifetime (seconds)")]
        public int? ConsentLifetime { get; set; }

        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Frontchannel Logout session required")]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Frontchannel Logout URI")]
        public string FrontChannelLogoutUri { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Pairwise Subject Salt")]
        public string PairWiseSubjectSalt { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Device Code Lifetime (seconds)")]
        public int DeviceCodeLifetime { get; set; }

        [Display(Name = "Noneditable")]
        public bool NonEditable { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "User Code Type")]
        public string UserCodeType { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "User SSO Lifetime (seconds)")]
        public int? UserSsoLifetime { get; set; }

        [Required]
        public int AccessTokenType { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
        public DateTime LastAccessed { get; set; }
    }
}
