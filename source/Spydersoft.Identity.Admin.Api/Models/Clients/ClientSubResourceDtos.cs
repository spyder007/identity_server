using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Clients
{
    // ---- Claims ----
    /// <summary>
    /// Represents a claim associated with a client.
    /// </summary>
    public class ClientClaimDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the claim.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the type of the claim.</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the value of the claim.</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client claim.
    /// </summary>
    public class SaveClientClaimDto
    {
        /// <summary>Gets or sets the type of the claim.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the value of the claim.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }

    // ---- Cors Origins ----
    /// <summary>
    /// Represents an allowed CORS origin for a client.
    /// </summary>
    public class ClientCorsOriginDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the CORS origin.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the allowed CORS origin.</summary>
        public string Origin { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client CORS origin.
    /// </summary>
    public class SaveClientCorsOriginDto
    {
        /// <summary>Gets or sets the allowed CORS origin.</summary>
        [Required][StringLength(150, MinimumLength = 1)] public string Origin { get; set; } = string.Empty;
    }

    // ---- Grant Types ----
    /// <summary>
    /// Represents a grant type allowed for a client.
    /// </summary>
    public class ClientGrantTypeDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the grant type.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the allowed grant type.</summary>
        public string GrantType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client grant type.
    /// </summary>
    public class SaveClientGrantTypeDto
    {
        /// <summary>Gets or sets the allowed grant type.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string GrantType { get; set; } = string.Empty;
    }

    // ---- IDP Restrictions ----
    /// <summary>
    /// Represents an identity provider restriction for a client.
    /// </summary>
    public class ClientIdpRestrictionDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the identity provider restriction.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the identity provider the client is restricted to.</summary>
        public string Provider { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client identity provider restriction.
    /// </summary>
    public class SaveClientIdpRestrictionDto
    {
        /// <summary>Gets or sets the identity provider the client is restricted to.</summary>
        [Required][StringLength(200, MinimumLength = 1)] public string Provider { get; set; } = string.Empty;
    }

    // ---- Post Logout Redirect URIs ----
    /// <summary>
    /// Represents a post-logout redirect URI for a client.
    /// </summary>
    public class ClientPostLogoutRedirectUriDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the post-logout redirect URI.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the post-logout redirect URI.</summary>
        public string PostLogoutRedirectUri { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client post-logout redirect URI.
    /// </summary>
    public class SaveClientPostLogoutRedirectUriDto
    {
        /// <summary>Gets or sets the post-logout redirect URI.</summary>
        [Required][StringLength(2000, MinimumLength = 1)] public string PostLogoutRedirectUri { get; set; } = string.Empty;
    }

    // ---- Properties ----
    /// <summary>
    /// Represents a custom property associated with a client.
    /// </summary>
    public class ClientPropertyDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the property.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the key of the property.</summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the value of the property.</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client property.
    /// </summary>
    public class SaveClientPropertyDto
    {
        /// <summary>Gets or sets the key of the property.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        /// <summary>Gets or sets the value of the property.</summary>
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }

    // ---- Redirect URIs ----
    /// <summary>
    /// Represents a redirect URI for a client.
    /// </summary>
    public class ClientRedirectUriDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the redirect URI.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the redirect URI.</summary>
        public string RedirectUri { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client redirect URI.
    /// </summary>
    public class SaveClientRedirectUriDto
    {
        /// <summary>Gets or sets the redirect URI.</summary>
        [Required][StringLength(2000, MinimumLength = 1)] public string RedirectUri { get; set; } = string.Empty;
    }

    // ---- Scopes ----
    /// <summary>
    /// Represents a scope granted to a client.
    /// </summary>
    public class ClientScopeDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the scope.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the scope granted to the client.</summary>
        public string Scope { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the data required to create or update a client scope.
    /// </summary>
    public class SaveClientScopeDto
    {
        /// <summary>Gets or sets the scope granted to the client.</summary>
        [Required][StringLength(200, MinimumLength = 1)] public string Scope { get; set; } = string.Empty;
    }

    // ---- Secrets ----
    /// <summary>
    /// Represents a secret associated with a client.
    /// </summary>
    public class ClientSecretDto : BaseApiDto
    {
        /// <summary>Gets or sets the identifier of the client that owns the secret.</summary>
        public int ClientId { get; set; }
        /// <summary>Gets or sets the type of the secret.</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>Gets or sets the description of the secret.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets the expiration of the secret.</summary>
        public string? Expiration { get; set; }
    }

    /// <summary>
    /// Represents the data required to create or update a client secret.
    /// </summary>
    public class SaveClientSecretDto
    {
        /// <summary>Gets or sets the type of the secret.</summary>
        [Required][StringLength(250, MinimumLength = 1)] public string Type { get; set; } = "SharedSecret";
        /// <summary>Gets or sets the value of the secret.</summary>
        [Required][StringLength(4000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
        /// <summary>Gets or sets the description of the secret.</summary>
        public string? Description { get; set; }
        /// <summary>Gets or sets the expiration of the secret.</summary>
        public string? Expiration { get; set; }
    }
}
