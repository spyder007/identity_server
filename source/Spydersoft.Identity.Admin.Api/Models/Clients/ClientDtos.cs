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
        /// <summary>Gets or sets the human-readable description of the client.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the client home page URI.</summary>
        public string? ClientUri { get; set; }

        /// <summary>Gets or sets the URI to the client logo image.</summary>
        public string? LogoUri { get; set; }

        /// <summary>Gets or sets a value indicating whether a client secret is required to obtain tokens.</summary>
        public bool RequireClientSecret { get; set; }

        /// <summary>Gets or sets a value indicating whether the user must give consent before tokens are issued.</summary>
        public bool RequireConsent { get; set; }

        /// <summary>Gets or sets a value indicating whether Proof Key for Code Exchange (PKCE) is required for authorization code flow.</summary>
        public bool RequirePkce { get; set; }

        /// <summary>Gets or sets a value indicating whether plain-text PKCE code challenges are allowed.</summary>
        public bool AllowPlainTextPkce { get; set; }

        /// <summary>Gets or sets a value indicating whether the client can request refresh tokens for offline access.</summary>
        public bool AllowOfflineAccess { get; set; }

        /// <summary>Gets or sets a value indicating whether access tokens may be transmitted via the browser.</summary>
        public bool AllowAccessTokensViaBrowser { get; set; }

        /// <summary>Gets or sets a value indicating whether prior consent decisions can be remembered for this client.</summary>
        public bool AllowRememberConsent { get; set; }

        /// <summary>Gets or sets a value indicating whether client claims are sent for every token request, not just client credentials flows.</summary>
        public bool AlwaysSendClientClaims { get; set; }

        /// <summary>Gets or sets a value indicating whether user claims are always included in the identity token.</summary>
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        /// <summary>Gets or sets a value indicating whether local login is enabled for this client.</summary>
        public bool EnableLocalLogin { get; set; }

        /// <summary>Gets or sets a value indicating whether a unique JWT identifier (jti) is included in tokens.</summary>
        public bool IncludeJwtId { get; set; }

        /// <summary>Gets or sets a value indicating whether the client is read-only and cannot be edited.</summary>
        public bool NonEditable { get; set; }

        /// <summary>Gets or sets a value indicating whether access token claims are refreshed when a refresh token is used.</summary>
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        /// <summary>Gets or sets a value indicating whether a session identifier is sent on back-channel logout requests.</summary>
        public bool BackChannelLogoutSessionRequired { get; set; }

        /// <summary>Gets or sets a value indicating whether a session identifier is sent on front-channel logout requests.</summary>
        public bool FrontChannelLogoutSessionRequired { get; set; }

        /// <summary>Gets or sets the maximum lifetime, in seconds, of a refresh token regardless of usage.</summary>
        public int AbsoluteRefreshTokenLifetime { get; set; }

        /// <summary>Gets or sets the access token lifetime in seconds.</summary>
        public int AccessTokenLifetime { get; set; }

        /// <summary>Gets or sets the access token type (0 = JWT, 1 = reference).</summary>
        public int AccessTokenType { get; set; }

        /// <summary>Gets or sets the authorization code lifetime in seconds.</summary>
        public int AuthorizationCodeLifetime { get; set; }

        /// <summary>Gets or sets the device code lifetime in seconds.</summary>
        public int DeviceCodeLifetime { get; set; }

        /// <summary>Gets or sets the identity token lifetime in seconds.</summary>
        public int IdentityTokenLifetime { get; set; }

        /// <summary>Gets or sets the refresh token expiration type (0 = sliding, 1 = absolute).</summary>
        public int RefreshTokenExpiration { get; set; }

        /// <summary>Gets or sets the refresh token usage type (0 = reuse, 1 = one-time only).</summary>
        public int RefreshTokenUsage { get; set; }

        /// <summary>Gets or sets the sliding refresh token lifetime in seconds.</summary>
        public int SlidingRefreshTokenLifetime { get; set; }

        /// <summary>Gets or sets the lifetime of a remembered consent, in seconds, or <c>null</c> for no expiration.</summary>
        public int? ConsentLifetime { get; set; }

        /// <summary>Gets or sets the maximum single sign-on session lifetime for the user, in seconds, or <c>null</c> for the global default.</summary>
        public int? UserSsoLifetime { get; set; }

        /// <summary>Gets or sets the URI invoked to notify the client of a back-channel logout.</summary>
        public string? BackChannelLogoutUri { get; set; }

        /// <summary>Gets or sets the prefix applied to client claim types.</summary>
        public string? ClientClaimsPrefix { get; set; }

        /// <summary>Gets or sets the URI invoked to notify the client of a front-channel logout.</summary>
        public string? FrontChannelLogoutUri { get; set; }

        /// <summary>Gets or sets the salt used to generate a pairwise subject identifier.</summary>
        public string? PairWiseSubjectSalt { get; set; }

        /// <summary>Gets or sets the user code type used for device flow.</summary>
        public string? UserCodeType { get; set; }

        /// <summary>Gets or sets the timestamp of the last update.</summary>
        public DateTime Updated { get; set; }

        /// <summary>Gets or sets the timestamp the client was last accessed.</summary>
        public DateTime LastAccessed { get; set; }
    }

    /// <summary>Payload for creating or updating a client.</summary>
    public class SaveClientDto
    {
        /// <summary>Gets or sets the client identifier string.</summary>
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string ClientId { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name.</summary>
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string ClientName { get; set; } = string.Empty;

        /// <summary>Gets or sets the human-readable description of the client.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the client home page URI.</summary>
        public string? ClientUri { get; set; }

        /// <summary>Gets or sets the URI to the client logo image.</summary>
        public string? LogoUri { get; set; }

        /// <summary>Gets or sets a value indicating whether a client secret is required to obtain tokens.</summary>
        public bool RequireClientSecret { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether the user must give consent before tokens are issued.</summary>
        public bool RequireConsent { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether Proof Key for Code Exchange (PKCE) is required for authorization code flow.</summary>
        public bool RequirePkce { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether plain-text PKCE code challenges are allowed.</summary>
        public bool? AllowPlainTextPkce { get; set; }

        /// <summary>Gets or sets a value indicating whether the client can request refresh tokens for offline access.</summary>
        public bool? AllowOfflineAccess { get; set; }

        /// <summary>Gets or sets a value indicating whether access tokens may be transmitted via the browser.</summary>
        public bool? AllowAccessTokensViaBrowser { get; set; }

        /// <summary>Gets or sets a value indicating whether prior consent decisions can be remembered for this client.</summary>
        public bool AllowRememberConsent { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether client claims are sent for every token request, not just client credentials flows.</summary>
        public bool? AlwaysSendClientClaims { get; set; }

        /// <summary>Gets or sets a value indicating whether user claims are always included in the identity token.</summary>
        public bool? AlwaysIncludeUserClaimsInIdToken { get; set; }

        /// <summary>Gets or sets a value indicating whether local login is enabled for this client.</summary>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether a unique JWT identifier (jti) is included in tokens.</summary>
        public bool? IncludeJwtId { get; set; }

        /// <summary>Gets or sets a value indicating whether the client is read-only and cannot be edited.</summary>
        public bool? NonEditable { get; set; }

        /// <summary>Gets or sets a value indicating whether access token claims are refreshed when a refresh token is used.</summary>
        public bool? UpdateAccessTokenClaimsOnRefresh { get; set; }

        /// <summary>Gets or sets a value indicating whether a session identifier is sent on back-channel logout requests.</summary>
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether a session identifier is sent on front-channel logout requests.</summary>
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether this client is enabled.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Gets or sets the maximum lifetime, in seconds, of a refresh token regardless of usage.</summary>
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        /// <summary>Gets or sets the access token lifetime in seconds.</summary>
        public int AccessTokenLifetime { get; set; } = 3600;

        /// <summary>Gets or sets the access token type (0 = JWT, 1 = reference).</summary>
        public int? AccessTokenType { get; set; }

        /// <summary>Gets or sets the authorization code lifetime in seconds.</summary>
        public int AuthorizationCodeLifetime { get; set; } = 300;

        /// <summary>Gets or sets the device code lifetime in seconds.</summary>
        public int DeviceCodeLifetime { get; set; } = 300;

        /// <summary>Gets or sets the identity token lifetime in seconds.</summary>
        public int IdentityTokenLifetime { get; set; } = 300;

        /// <summary>Gets or sets the refresh token expiration type (0 = sliding, 1 = absolute).</summary>
        public int? RefreshTokenExpiration { get; set; }

        /// <summary>Gets or sets the refresh token usage type (0 = reuse, 1 = one-time only).</summary>
        public int? RefreshTokenUsage { get; set; }

        /// <summary>Gets or sets the sliding refresh token lifetime in seconds.</summary>
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        /// <summary>Gets or sets the lifetime of a remembered consent, in seconds, or <c>null</c> for no expiration.</summary>
        public int? ConsentLifetime { get; set; }

        /// <summary>Gets or sets the maximum single sign-on session lifetime for the user, in seconds, or <c>null</c> for the global default.</summary>
        public int? UserSsoLifetime { get; set; }

        /// <summary>Gets or sets the URI invoked to notify the client of a back-channel logout.</summary>
        public string? BackChannelLogoutUri { get; set; }

        /// <summary>Gets or sets the prefix applied to client claim types.</summary>
        public string? ClientClaimsPrefix { get; set; }

        /// <summary>Gets or sets the URI invoked to notify the client of a front-channel logout.</summary>
        public string? FrontChannelLogoutUri { get; set; }

        /// <summary>Gets or sets the salt used to generate a pairwise subject identifier.</summary>
        public string? PairWiseSubjectSalt { get; set; }

        /// <summary>Gets or sets the protocol type.</summary>
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string ProtocolType { get; set; } = "oidc";

        /// <summary>Gets or sets the user code type used for device flow.</summary>
        public string? UserCodeType { get; set; }
    }
}
