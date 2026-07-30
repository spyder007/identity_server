// Known-value lists used to back dropdowns for fields that are otherwise free text.
// These are the standard sets defined by the underlying protocol/libraries, not
// backed by an API endpoint, so they're kept here as static, editable-dropdown data.

// Matches Duende.IdentityServer.IdentityServerConstants.GrantTypes (the individual
// grant type strings clients are allowed to be assigned, one row per grant type).
export const GRANT_TYPES = [
  "authorization_code",
  "client_credentials",
  "hybrid",
  "implicit",
  "password",
  "urn:ietf:params:oauth:grant-type:device_code",
  "urn:openid:params:grant-type:ciba",
];

// Duende's built-in "local" IdP restriction value (IdentityServerConstants.LocalIdentityProvider)
// plus the external providers wired up in Spydersoft.Identity/Program.cs. Kept as an
// editable list since IdP restrictions store an arbitrary authentication scheme name.
export const IDP_PROVIDERS = ["local", "Google"];

// Common claim types from Duende.IdentityModel.JwtClaimTypes, the set already used
// when seeding user claims (see Spydersoft.Identity.DataSeeder/Seeding/Identity.cs).
// Kept as an editable list since any string is accepted server-side.
export const CLAIM_TYPES = [
  "sub",
  "name",
  "given_name",
  "family_name",
  "middle_name",
  "nickname",
  "preferred_username",
  "profile",
  "picture",
  "website",
  "email",
  "email_verified",
  "gender",
  "birthdate",
  "zoneinfo",
  "locale",
  "phone_number",
  "phone_number_verified",
  "address",
  "updated_at",
  "role",
];
