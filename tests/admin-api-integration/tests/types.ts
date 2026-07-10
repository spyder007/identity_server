/**
 * Hand-rolled subsets of the Admin.Api DTOs. The full shapes live in
 * Spydersoft.Identity.Admin.Api/Models/*. Keep this file minimal — only the
 * fields the integration tests assert on or post.
 *
 * Note: most string fields have MinLength=2 on the server. Generate test
 * values via uniqueSuffix() (>2 chars) and prefix any human-meaningful
 * portion with enough letters to clear the minimum.
 */

export interface ClientSummary {
  id: number;
  clientId: string;
  clientName: string | null;
  enabled: boolean;
  protocolType: string;
}

export interface ClientDetail extends ClientSummary {
  description: string | null;
  requirePkce: boolean;
}

export interface SaveClient {
  clientId: string;
  clientName: string;
  protocolType: string;
}

export interface ApiResourceSummary {
  id: number;
  name: string;
  displayName: string | null;
  enabled: boolean;
}

export interface SaveApiResource {
  name: string;
  displayName?: string | null;
  description?: string | null;
  enabled?: boolean;
  nonEditable?: boolean;
}

export interface IdentityResourceSummary {
  id: number;
  name: string;
  displayName: string | null;
  enabled: boolean;
}

export interface SaveIdentityResource {
  name: string;
  displayName?: string | null;
  description?: string | null;
  enabled?: boolean;
  required?: boolean;
  emphasize?: boolean;
  showInDiscoveryDocument?: boolean;
  nonEditable?: boolean;
}

export interface ScopeSummary {
  id: number;
  name: string;
  displayName: string | null;
  enabled: boolean;
}

export interface SaveScope {
  name: string;
  displayName?: string | null;
  description?: string | null;
  enabled?: boolean;
  required?: boolean;
  emphasize?: boolean;
  showInDiscoveryDocument?: boolean;
}

export interface UserSummary {
  id: string;
  userName: string | null;
  email: string | null;
  emailConfirmed: boolean;
  name: string | null;
  twoFactorEnabled: boolean;
  lockoutEnabled: boolean;
  accessFailedCount: number;
}

export interface UserDetail extends UserSummary {
  phoneNumber: string | null;
  phoneNumberConfirmed: boolean;
}

export interface CreateUser {
  password: string;
  userName: string;
  email: string;
  name?: string | null;
  phoneNumber?: string | null;
  twoFactorEnabled?: boolean;
  lockoutEnabled?: boolean;
}

export type SaveUser = Omit<CreateUser, 'password'>;

export interface RoleSummary {
  id: string;
  name: string | null;
}

export interface SaveRole {
  name: string;
}

export interface UserRole {
  roleName: string;
}

export interface RoleClaim {
  type: string;
  value: string;
}

export interface UserClaim {
  type: string;
  value: string;
}

export function uniqueSuffix(): string {
  return crypto.randomUUID().replaceAll('-', '').slice(0, 12);
}
