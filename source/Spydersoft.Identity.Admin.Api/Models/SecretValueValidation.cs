using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

using Duende.IdentityServer;

namespace Spydersoft.Identity.Admin.Api.Models
{
    /// <summary>
    /// Shared validation for client/API resource secrets across all <c>IdentityServerConstants.SecretTypes</c>
    /// values. The Admin API is the only place secrets get provisioned, so this is the single source of truth for
    /// what makes a secret value acceptable for a given type.
    /// </summary>
    internal static partial class SecretValueValidation
    {
        private static readonly HashSet<string> AllowedTypes =
        [
            IdentityServerConstants.SecretTypes.SharedSecret,
            IdentityServerConstants.SecretTypes.X509CertificateThumbprint,
            IdentityServerConstants.SecretTypes.X509CertificateName,
            IdentityServerConstants.SecretTypes.X509CertificateBase64,
        ];

        [GeneratedRegex("^[0-9A-Fa-f]{40}$")]
        private static partial Regex ThumbprintRegex();

        [GeneratedRegex(@"^[^=,]+=[^=,]+(\s*,\s*[^=,]+=[^=,]+)*$")]
        private static partial Regex DistinguishedNameRegex();

        /// <summary>Validates a secret's type/value pair, yielding one result per problem found.</summary>
        public static IEnumerable<ValidationResult> Validate(string type, string value)
        {
            if (!AllowedTypes.Contains(type))
            {
                yield return new ValidationResult(
                    $"Type must be one of: {string.Join(", ", AllowedTypes)}.",
                    ["Type"]);
                yield break;
            }

            if (type == IdentityServerConstants.SecretTypes.X509CertificateThumbprint && !ThumbprintRegex().IsMatch(value))
            {
                yield return new ValidationResult(
                    "Thumbprint must be exactly 40 hexadecimal characters (the certificate's SHA-1 thumbprint).",
                    ["Value"]);
            }

            if (type == IdentityServerConstants.SecretTypes.X509CertificateName && !DistinguishedNameRegex().IsMatch(value.Trim()))
            {
                yield return new ValidationResult(
                    "Name must be a distinguished name, e.g. CN=MyClient, O=MyOrg.",
                    ["Value"]);
            }

            if (type == IdentityServerConstants.SecretTypes.X509CertificateBase64 && !TryParseCertificate(value))
            {
                yield return new ValidationResult(
                    "Value must be a valid Base64-encoded X.509 certificate.",
                    ["Value"]);
            }
        }

        private static bool TryParseCertificate(string value)
        {
            try
            {
                var bytes = Convert.FromBase64String(value);
                using var certificate = X509CertificateLoader.LoadCertificate(bytes);
                return true;
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException or ArgumentException)
            {
                return false;
            }
        }
    }
}
