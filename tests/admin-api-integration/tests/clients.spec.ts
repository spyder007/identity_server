import { test, expect } from '../fixtures';
import type { ClientDetail, ClientSummary, SaveClient } from './types';
import { uniqueSuffix } from './types';

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
