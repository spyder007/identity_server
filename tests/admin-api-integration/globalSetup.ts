import { request as playwrightRequest } from '@playwright/test';
import { mkdirSync, writeFileSync } from 'node:fs';
import path from 'node:path';

// 127.0.0.1, not localhost — Node on Windows often resolves localhost to ::1
// (IPv6) while Kestrel binds IPv4 only for `http://localhost:7020` style URLs,
// producing ECONNREFUSED ::1:7020. Pin to v4 to avoid the mismatch.
const authority = process.env.IDENTITY_AUTHORITY ?? 'http://127.0.0.1:7020';
const clientId = process.env.IDENTITY_ADMIN_TESTS_CLIENT_ID ?? 'identity.admin.tests';
// Cleartext secret matches Spydersoft.Identity.DataSeeder.Seeding.Clients
// .AdminTestsClientSecret. Test-only credential, committed deliberately.
const clientSecret =
  process.env.IDENTITY_ADMIN_TESTS_CLIENT_SECRET ?? 'local-tests-only-do-not-deploy';
const scope = 'identity:admin:read identity:admin:write';

export const tokenFile = path.join(__dirname, '.auth', 'token.json');

async function waitForDiscovery(ctx: import('@playwright/test').APIRequestContext) {
  // Even with WaitFor(identity), Aspire only confirms the resource has started,
  // not that Kestrel is serving discovery yet. Poll up to 90s.
  const deadline = Date.now() + 90_000;
  let lastError = '';
  while (Date.now() < deadline) {
    try {
      const r = await ctx.get('/.well-known/openid-configuration', { timeout: 5_000 });
      if (r.ok()) return;
      lastError = `${r.status()} ${r.statusText()}`;
    } catch (e) {
      lastError = (e as Error).message;
    }
    await new Promise((resolve) => setTimeout(resolve, 2_000));
  }
  throw new Error(
    `Identity discovery endpoint at ${authority} not ready after 90s. Last error: ${lastError}`,
  );
}

export default async function globalSetup() {
  const ctx = await playwrightRequest.newContext({ baseURL: authority });
  await waitForDiscovery(ctx);

  const form = new URLSearchParams({
    grant_type: 'client_credentials',
    client_id: clientId,
    client_secret: clientSecret,
    scope,
  });

  const response = await ctx.post('/connect/token', {
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    data: form.toString(),
  });

  if (!response.ok()) {
    const body = await response.text();
    throw new Error(
      `Token request failed (${response.status()} ${response.statusText()}): ${body}`,
    );
  }

  const json = (await response.json()) as { access_token?: string; expires_in?: number };
  if (!json.access_token) {
    throw new Error(`Token response missing access_token: ${JSON.stringify(json)}`);
  }

  mkdirSync(path.dirname(tokenFile), { recursive: true });
  writeFileSync(tokenFile, JSON.stringify({ token: json.access_token, expiresIn: json.expires_in }));
  await ctx.dispose();
}
