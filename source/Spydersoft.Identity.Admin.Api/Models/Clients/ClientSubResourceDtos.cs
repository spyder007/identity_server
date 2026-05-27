using System.ComponentModel.DataAnnotations;

namespace Spydersoft.Identity.Admin.Api.Models.Clients
{
    // ---- Claims ----
    public class ClientClaimDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SaveClientClaimDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Type { get; set; } = string.Empty;
        [Required][StringLength(250, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }

    // ---- Cors Origins ----
    public class ClientCorsOriginDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string Origin { get; set; } = string.Empty;
    }

    public class SaveClientCorsOriginDto
    {
        [Required][StringLength(150, MinimumLength = 1)] public string Origin { get; set; } = string.Empty;
    }

    // ---- Grant Types ----
    public class ClientGrantTypeDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string GrantType { get; set; } = string.Empty;
    }

    public class SaveClientGrantTypeDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string GrantType { get; set; } = string.Empty;
    }

    // ---- IDP Restrictions ----
    public class ClientIdpRestrictionDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string Provider { get; set; } = string.Empty;
    }

    public class SaveClientIdpRestrictionDto
    {
        [Required][StringLength(200, MinimumLength = 1)] public string Provider { get; set; } = string.Empty;
    }

    // ---- Post Logout Redirect URIs ----
    public class ClientPostLogoutRedirectUriDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string PostLogoutRedirectUri { get; set; } = string.Empty;
    }

    public class SaveClientPostLogoutRedirectUriDto
    {
        [Required][StringLength(2000, MinimumLength = 1)] public string PostLogoutRedirectUri { get; set; } = string.Empty;
    }

    // ---- Properties ----
    public class ClientPropertyDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SaveClientPropertyDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Key { get; set; } = string.Empty;
        [Required][StringLength(2000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
    }

    // ---- Redirect URIs ----
    public class ClientRedirectUriDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string RedirectUri { get; set; } = string.Empty;
    }

    public class SaveClientRedirectUriDto
    {
        [Required][StringLength(2000, MinimumLength = 1)] public string RedirectUri { get; set; } = string.Empty;
    }

    // ---- Scopes ----
    public class ClientScopeDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string Scope { get; set; } = string.Empty;
    }

    public class SaveClientScopeDto
    {
        [Required][StringLength(200, MinimumLength = 1)] public string Scope { get; set; } = string.Empty;
    }

    // ---- Secrets ----
    public class ClientSecretDto : BaseApiDto
    {
        public int ClientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Expiration { get; set; }
    }

    public class SaveClientSecretDto
    {
        [Required][StringLength(250, MinimumLength = 1)] public string Type { get; set; } = "SharedSecret";
        [Required][StringLength(4000, MinimumLength = 1)] public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Expiration { get; set; }
    }
}
