namespace Spydersoft.Identity.Admin.Api;

/// <summary>Authorization policy names for the Admin API.</summary>
public static class AdminApiPolicies
{
    /// <summary>Policy for read-only operations (GET). Accepts read or write scope.</summary>
    public const string Read = "identity:admin:read";

    /// <summary>Policy for mutating operations (POST, PUT, DELETE). Requires write scope.</summary>
    public const string Write = "identity:admin:write";
}
