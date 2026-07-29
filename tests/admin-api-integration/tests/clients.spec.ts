import { test, expect } from '../fixtures';
import type { ClientDetail, ClientSecretDetail, ClientSummary, SaveClient, SaveClientSecret } from './types';
import { uniqueSuffix } from './types';

// The Testing launch profile runs identity over plain HTTP on this port (see
// tests/admin-ui-e2e/playwright.config.ts for the full port map). The admin-api
// `request` fixture is scoped to the Admin.Api baseURL, so the token-endpoint
// test below needs identity's own origin.
const identityBaseUrl = process.env.IDENTITY_BASE_URL ?? 'http://localhost:7020';

function payload(): SaveClient {
  return {
    clientId: `tests.client.${uniqueSuffix()}`,
    clientName: `Test Client ${uniqueSuffix()}`,
    protocolType: 'oidc',
  };
}

async function create(request: import('@playwright/test').APIRequestContext) {
  const body = payload();
  const response = await request.post('/api/v1/Clients', { data: body });
  expect(response.status(), 'create client').toBe(201);
  const detail = (await response.json()) as ClientDetail;
  expect(detail.id).toBeGreaterThan(0);
  return { body, detail };
}

test('clients.list returns the seeded clients', async ({ request }) => {
  const response = await request.get('/api/v1/Clients');
  expect(response.status()).toBe(200);
  const list = (await response.json()) as ClientSummary[];
  expect(Array.isArray(list)).toBe(true);
  // The seeder always creates identity.admin.frontend
  expect(list.some((c) => c.clientId === 'identity.admin.frontend')).toBe(true);
});

test('clients.create then get returns matching fields', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const response = await request.get(`/api/v1/Clients/${detail.id}`);
    expect(response.status()).toBe(200);
    const got = (await response.json()) as ClientDetail;
    expect(got.clientId).toBe(body.clientId);
    expect(got.clientName).toBe(body.clientName);
    expect(got.protocolType).toBe('oidc');
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});

test('clients.update mutates the record', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const updated: SaveClient = { ...body, clientName: `Renamed ${uniqueSuffix()}` };
    const put = await request.put(`/api/v1/Clients/${detail.id}`, { data: updated });
    expect(put.status()).toBe(204);

    const got = await (await request.get(`/api/v1/Clients/${detail.id}`)).json();
    expect((got as ClientDetail).clientName).toBe(updated.clientName);
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});

test('clients.delete removes the record', async ({ request }) => {
  const { detail } = await create(request);
  const del = await request.delete(`/api/v1/Clients/${detail.id}`);
  expect(del.status()).toBe(204);
  const got = await request.get(`/api/v1/Clients/${detail.id}`);
  expect(got.status()).toBe(404);
});

test('clients.scopes sub-resource round-trip', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const scopeName = `tests.scope.${uniqueSuffix()}`;
    const post = await request.post(`/api/v1/clients/${detail.id}/scopes`, {
      data: { scope: scopeName },
    });
    expect([200, 201, 204]).toContain(post.status());

    const list = await request.get(`/api/v1/clients/${detail.id}/scopes`);
    expect(list.status()).toBe(200);
    const scopes = (await list.json()) as Array<{ id: number; scope: string }>;
    const row = scopes.find((s) => s.scope === scopeName);
    expect(row, 'newly added scope should appear in list').toBeDefined();

    if (row) {
      const del = await request.delete(`/api/v1/clients/${detail.id}/scopes/${row.id}`);
      expect(del.status()).toBe(204);
    }
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});

// Regression test for the bug where SharedSecret values were persisted
// verbatim instead of SHA-256 hashed, which made every client using one fail
// at the token endpoint with "invalid hashing algorithm". The Save/summary
// DTOs never echo Value back, so the strongest proof available over the API
// surface that the plaintext value got hashed the way IdentityServer's
// HashedSharedSecretValidator expects is that the client can actually use it
// to obtain a token.
test('clients.secrets: a SharedSecret added via the API authenticates via client_credentials', async ({
  request,
}) => {
  const { body, detail } = await create(request);
  try {
    const grantType = await request.post(`/api/v1/clients/${detail.id}/granttypes`, {
      data: { grantType: 'client_credentials' },
    });
    expect(grantType.status(), 'assign client_credentials grant type').toBe(201);

    const scope = await request.post(`/api/v1/clients/${detail.id}/scopes`, {
      data: { scope: 'identity:admin:read' },
    });
    expect(scope.status(), 'assign identity:admin:read scope').toBe(201);

    const plaintext = `S3cret.${uniqueSuffix()}`;
    const post = await request.post(`/api/v1/clients/${detail.id}/secrets`, {
      data: { type: 'SharedSecret', value: plaintext } satisfies SaveClientSecret,
    });
    expect(post.status(), 'create secret').toBe(201);
    const created = (await post.json()) as ClientSecretDetail;
    expect(created.type).toBe('SharedSecret');

    const list = await request.get(`/api/v1/clients/${detail.id}/secrets`);
    const secrets = (await list.json()) as ClientSecretDetail[];
    expect(secrets.some((s) => s.id === created.id), 'newly added secret should appear in list').toBe(true);

    const token = await request.post(`${identityBaseUrl}/connect/token`, {
      form: {
        grant_type: 'client_credentials',
        client_id: body.clientId,
        client_secret: plaintext,
        scope: 'identity:admin:read',
      },
    });
    expect(token.status(), 'token request with the plaintext secret').toBe(200);
    const tokenBody = (await token.json()) as { access_token?: string };
    expect(tokenBody.access_token).toBeTruthy();
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});

test('clients.secrets: rejects a malformed X509 thumbprint but accepts a well-formed one', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const bad = await request.post(`/api/v1/clients/${detail.id}/secrets`, {
      data: { type: 'X509Thumbprint', value: 'not-a-thumbprint' } satisfies SaveClientSecret,
    });
    expect(bad.status()).toBe(400);

    // Format is all the server can check at creation time (the real thumbprint
    // match happens against the presented client certificate at token time) —
    // 40 hex characters is the only thing that makes a value well-formed.
    const good = await request.post(`/api/v1/clients/${detail.id}/secrets`, {
      data: { type: 'X509Thumbprint', value: '0'.repeat(40) } satisfies SaveClientSecret,
    });
    expect(good.status()).toBe(201);
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});

test('clients.secrets: rejects a non-certificate X509CertificateBase64 value', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const bad = await request.post(`/api/v1/clients/${detail.id}/secrets`, {
      data: { type: 'X509CertificateBase64', value: 'not valid base64!!' } satisfies SaveClientSecret,
    });
    expect(bad.status()).toBe(400);
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});

test('clients.secrets: rejects an unrecognized secret type', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const bad = await request.post(`/api/v1/clients/${detail.id}/secrets`, {
      data: { type: 'NotARealSecretType', value: 'whatever' } satisfies SaveClientSecret,
    });
    expect(bad.status()).toBe(400);
  } finally {
    await request.delete(`/api/v1/Clients/${detail.id}`);
  }
});
