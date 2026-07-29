// Matches Duende.IdentityServer.IdentityServerConstants.SecretTypes, and mirrors the server-side
// validation in Spydersoft.Identity.Admin.Api/Models/SecretValueValidation.cs. Only SharedSecret
// values get hashed server-side; the X509 types store the raw thumbprint/name/cert value.
export interface SecretTypeOption {
  label: string;
  value: string;
  valueHint: string;
  placeholder: string;
  multiline: boolean;
}

export const SECRET_TYPES: SecretTypeOption[] = [
  {
    label: "Shared secret",
    value: "SharedSecret",
    valueHint: " (will be SHA-256 hashed)",
    placeholder: "Plaintext secret value",
    multiline: false,
  },
  {
    label: "X509 certificate thumbprint",
    value: "X509Thumbprint",
    valueHint: "",
    placeholder: "SHA-1 thumbprint, e.g. 8F3B5A...",
    multiline: false,
  },
  {
    label: "X509 certificate name",
    value: "X509Name",
    valueHint: "",
    placeholder: "Subject distinguished name, e.g. CN=MyClient, O=MyOrg",
    multiline: false,
  },
  {
    label: "X509 certificate (base64)",
    value: "X509CertificateBase64",
    valueHint: "",
    placeholder: "Base64-encoded DER certificate (public cert only, no private key)",
    multiline: true,
  },
];

const THUMBPRINT_PATTERN = /^[0-9A-Fa-f]{40}$/;
const DISTINGUISHED_NAME_PATTERN = /^[^=,]+=[^=,]+(\s*,\s*[^=,]+=[^=,]+)*$/;

// Fast client-side pre-check so obviously-bad values don't round-trip to the server.
// The Admin API re-validates authoritatively (it's the only thing that provisions secrets),
// including actually parsing the X509CertificateBase64 value as a certificate.
export function validateSecretValue(type: string, value: string): string | null {
  const trimmed = value.trim();
  switch (type) {
    case "X509Thumbprint":
      return THUMBPRINT_PATTERN.test(trimmed)
        ? null
        : "Thumbprint must be exactly 40 hexadecimal characters (the certificate's SHA-1 thumbprint).";
    case "X509Name":
      return DISTINGUISHED_NAME_PATTERN.test(trimmed)
        ? null
        : "Name must be a distinguished name, e.g. CN=MyClient, O=MyOrg.";
    case "X509CertificateBase64":
      try {
        atob(trimmed);
        return null;
      } catch {
        return "Value must be valid Base64.";
      }
    default:
      return null;
  }
}
