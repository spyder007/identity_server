using System;
using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Clients
{
    /// <summary>Summary DTO returned in list responses.</summary>
    public class ClientSummaryDto
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the client identifier string.</summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name.</summary>
        public string? ClientName { get; set; }

        /// <summary>Gets or sets a value indicating whether this client is enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets the protocol type.</summary>
        public string ProtocolType { get; set; } = "oidc";

        /// <summary>Gets or sets the creation timestamp.</summary>
        public DateTime Created { get; set; }
    }

    /// <summary>Full client detail DTO.</summary>
    public class ClientDto : ClientSummaryDto
    {
        public string? Description { get; set; }
        public string? ClientUri { get; set; }
        public string? LogoUri { get; set; }
        public bool RequireClientSecret { get; set; }
        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public bool AllowRememberConsent { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public bool EnableLocalLogin { get; set; }
        public bool IncludeJwtId { get; set; }
        public bool NonEditable { get; set; }
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; }
        public int AbsoluteRefreshTokenLifetime { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int AccessTokenType { get; set; }
        public int AuthorizationCodeLifetime { get; set; }
        public int DeviceCodeLifetime { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public int RefreshTokenUsage { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; }
        public int? ConsentLifetime { get; set; }
        public int? UserSsoLifetime { get; set; }
        public string? BackChannelLogoutUri { get; set; }
        public string? ClientClaimsPrefix { get; set; }
        public string? FrontChannelLogoutUri { get; set; }
        public string? PairWiseSubjectSalt { get; set; }
        public string? UserCodeType { get; set; }
        public DateTime Updated { get; set; }
        public DateTime LastAccessed { get; set; }
    }

    /// <summary>Payload for creating or updating a client.</summary>
    public class SaveClientDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string ClientName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? ClientUri { get; set; }
        public string? LogoUri { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public bool RequireConsent { get; set; } = true;
        public bool RequirePkce { get; set; } = true;
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysSendClientClaims { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public bool EnableLocalLogin { get; set; } = true;
        public bool IncludeJwtId { get; set; }
        public bool NonEditable { get; set; }
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AccessTokenType { get; set; }
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int DeviceCodeLifetime { get; set; } = 300;
        public int IdentityTokenLifetime { get; set; } = 300;
        public int RefreshTokenExpiration { get; set; }
        public int RefreshTokenUsage { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int? ConsentLifetime { get; set; }
        public int? UserSsoLifetime { get; set; }
        public string? BackChannelLogoutUri { get; set; }
        public string? ClientClaimsPrefix { get; set; }
        public string? FrontChannelLogoutUri { get; set; }
        public string? PairWiseSubjectSalt { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string ProtocolType { get; set; } = "oidc";
        public string? UserCodeType { get; set; }
    }
}
