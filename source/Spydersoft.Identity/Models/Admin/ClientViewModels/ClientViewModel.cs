using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientViewModel.
    /// Implements the <see cref="BaseAdminViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminViewModel" />
    public class ClientViewModel : BaseAdminViewModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientViewModel"/> class.
        /// </summary>
        public ClientViewModel()
        {
            NavBar = new NavBarViewModel(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientViewModel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public ClientViewModel(int id) : this()
        {
            Id = id;
            NavBar.Id = Id;
        }

        /// <summary>
        /// Gets or sets the nav bar.
        /// </summary>
        /// <value>The nav bar.</value>
        public NavBarViewModel NavBar { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>The name of the client.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the client URI.
        /// </summary>
        /// <value>The client URI.</value>
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Client URI")]
        public string ClientUri { get; set; }

        /// <summary>
        /// Gets or sets the absolute refresh token lifetime.
        /// </summary>
        /// <value>The absolute refresh token lifetime.</value>
        [Required]
        [Display(Name = "Absolute Refresh Token Lifetime (seconds)")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [DefaultValue(2592000)]
        public int AbsoluteRefreshTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets the access token lifetime.
        /// </summary>
        /// <value>The access token lifetime.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Access Token Lifetime (seconds)")]
        [DefaultValue(3600)]
        public int AccessTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow access tokens via browser].
        /// </summary>
        /// <value><c>true</c> if [allow access tokens via browser]; otherwise, <c>false</c>.</value>
        [Display(Name = "Allow access tokens via browser")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow offline access].
        /// </summary>
        /// <value><c>true</c> if [allow offline access]; otherwise, <c>false</c>.</value>
        [Display(Name = "Allow offline access")]
        public bool AllowOfflineAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow plain text pkce].
        /// </summary>
        /// <value><c>true</c> if [allow plain text pkce]; otherwise, <c>false</c>.</value>
        [Display(Name = "Allow plain text Pkce")]
        public bool AllowPlainTextPkce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow remember consent].
        /// </summary>
        /// <value><c>true</c> if [allow remember consent]; otherwise, <c>false</c>.</value>
        [Display(Name = "Allow remember consent")]
        public bool AllowRememberConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always send client claims].
        /// </summary>
        /// <value><c>true</c> if [always send client claims]; otherwise, <c>false</c>.</value>
        [Display(Name = "Always send client claims")]
        public bool AlwaysSendClientClaims { get; set; }

        /// <summary>
        /// Gets or sets the authorization code lifetime.
        /// </summary>
        /// <value>The authorization code lifetime.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Authorization Code Lifetime (seconds)")]
        [DefaultValue(300)]
        public int AuthorizationCodeLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable local login].
        /// </summary>
        /// <value><c>true</c> if [enable local login]; otherwise, <c>false</c>.</value>
        [Display(Name = "Enable local login")]
        public bool EnableLocalLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ClientViewModel"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [Display(Name = "Is Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the identity token lifetime.
        /// </summary>
        /// <value>The identity token lifetime.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Identity Token Lifetime (seconds)")]
        [DefaultValue(300)]
        public int IdentityTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include JWT identifier].
        /// </summary>
        /// <value><c>true</c> if [include JWT identifier]; otherwise, <c>false</c>.</value>
        [Display(Name = "Include JWT Id")]
        public bool IncludeJwtId { get; set; }

        /// <summary>
        /// Gets or sets the logo URI.
        /// </summary>
        /// <value>The logo URI.</value>
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Logo URI")]
        public string LogoUri { get; set; }

        /// <summary>
        /// Gets or sets the type of the protocol.
        /// </summary>
        /// <value>The type of the protocol.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Protocol Type")]
        [DefaultValue("oidc")]
        public string ProtocolType { get; set; }

        /// <summary>
        /// Gets or sets the refresh token expiration.
        /// </summary>
        /// <value>The refresh token expiration.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Refresh Token Expiration (seconds)")]
        public int RefreshTokenExpiration { get; set; }

        /// <summary>
        /// Gets or sets the refresh token usage.
        /// </summary>
        /// <value>The refresh token usage.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Refresh Token Usage")]
        public int RefreshTokenUsage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require client secret].
        /// </summary>
        /// <value><c>true</c> if [require client secret]; otherwise, <c>false</c>.</value>
        [Display(Name = "Require client secret")]
        public bool RequireClientSecret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require consent].
        /// </summary>
        /// <value><c>true</c> if [require consent]; otherwise, <c>false</c>.</value>
        [Display(Name = "Require consent")]
        public bool RequireConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require pkce].
        /// </summary>
        /// <value><c>true</c> if [require pkce]; otherwise, <c>false</c>.</value>
        [Display(Name = "Require PKCE")]
        public bool RequirePkce { get; set; }

        /// <summary>
        /// Gets or sets the sliding refresh token lifetime.
        /// </summary>
        /// <value>The sliding refresh token lifetime.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Sliding Refresh Token Lifetime (seconds)")]
        [DefaultValue(1296000)]
        public int SlidingRefreshTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [update access token claims on refresh].
        /// </summary>
        /// <value><c>true</c> if [update access token claims on refresh]; otherwise, <c>false</c>.</value>
        [Display(Name = "Update access token claims on refresh")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always include user claims in identifier token].
        /// </summary>
        /// <value><c>true</c> if [always include user claims in identifier token]; otherwise, <c>false</c>.</value>
        [Display(Name = "Always include user claims in ID Token")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [back channel logout session required].
        /// </summary>
        /// <value><c>true</c> if [back channel logout session required]; otherwise, <c>false</c>.</value>
        [Display(Name = "Backchannel Logout session required")]
        public bool BackChannelLogoutSessionRequired { get; set; }

        /// <summary>
        /// Gets or sets the back channel logout URI.
        /// </summary>
        /// <value>The back channel logout URI.</value>
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Backchannel Logout URI")]
        public string BackChannelLogoutUri { get; set; }

        /// <summary>
        /// Gets or sets the client claims prefix.
        /// </summary>
        /// <value>The client claims prefix.</value>
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Client claims prefix")]
        public string ClientClaimsPrefix { get; set; }

        /// <summary>
        /// Gets or sets the consent lifetime.
        /// </summary>
        /// <value>The consent lifetime.</value>
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Consent Lifetime (seconds)")]
        public int? ConsentLifetime { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [front channel logout session required].
        /// </summary>
        /// <value><c>true</c> if [front channel logout session required]; otherwise, <c>false</c>.</value>
        [Display(Name = "Frontchannel Logout session required")]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        /// <summary>
        /// Gets or sets the front channel logout URI.
        /// </summary>
        /// <value>The front channel logout URI.</value>
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Frontchannel Logout URI")]
        public string FrontChannelLogoutUri { get; set; }

        /// <summary>
        /// Gets or sets the pair wise subject salt.
        /// </summary>
        /// <value>The pair wise subject salt.</value>
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Pairwise Subject Salt")]
        public string PairWiseSubjectSalt { get; set; }

        /// <summary>
        /// Gets or sets the device code lifetime.
        /// </summary>
        /// <value>The device code lifetime.</value>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "Device Code Lifetime (seconds)")]
        public int DeviceCodeLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [non editable].
        /// </summary>
        /// <value><c>true</c> if [non editable]; otherwise, <c>false</c>.</value>
        [Display(Name = "Noneditable")]
        public bool NonEditable { get; set; }

        /// <summary>
        /// Gets or sets the type of the user code.
        /// </summary>
        /// <value>The type of the user code.</value>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "User Code Type")]
        public string UserCodeType { get; set; }

        /// <summary>
        /// Gets or sets the user sso lifetime.
        /// </summary>
        /// <value>The user sso lifetime.</value>
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        [Display(Name = "User SSO Lifetime (seconds)")]
        public int? UserSsoLifetime { get; set; }

        /// <summary>
        /// Gets or sets the type of the access token.
        /// </summary>
        /// <value>The type of the access token.</value>
        [Required]
        public int AccessTokenType { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the updated.
        /// </summary>
        /// <value>The updated.</value>
        public DateTime Updated { get; set; }
        /// <summary>
        /// Gets or sets the last accessed.
        /// </summary>
        /// <value>The last accessed.</value>
        public DateTime LastAccessed { get; set; }
    }
}