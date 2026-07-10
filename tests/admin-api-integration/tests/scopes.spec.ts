import { test, expect } from '../fixtures';
import type { SaveScope, ScopeSummary } from './types';
import { uniqueSuffix } from './types';

function payload(): SaveScope {
  return {
    name: `tests.scope.${uniqueSuffix()}`,
    displayName: `Test Scope ${uniqueSuffix()}`,
    description: `Created by admin-api-integration tests at ${new Date().toISOString()}`,
    enabled: true,
    required: false,
    emphasize: false,
    showInDiscoveryDocument: true,
  };
}

async function create(request: import('@playwright/test').APIRequestContext) {
  const body = payload();
  const response = await request.post('/api/v1/Scopes', { data: body });
  expect(response.status(), 'create scope').toBe(201);
  const detail = (await response.json()) as ScopeSummary;
  expect(detail.id).toBeGreaterThan(0);
  return { body, detail };
}

test('scopes.list returns the seeded admin scopes', async ({ request }) => {
  const response = await request.get('/api/v1/Scopes');
  expect(response.status()).toBe(200);
  const list = (await response.json()) as ScopeSummary[];
  for (const expected of ['identity:admin:read', 'identity:admin:write']) {
    expect(list.some((s) => s.name === expected)).toBe(true);
  }
});

test('scopes.create-get-update-delete round-trip', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const got = await (await request.get(`/api/v1/Scopes/${detail.id}`)).json();
    expect((got as ScopeSummary).name).toBe(body.name);

    const updated: SaveScope = { ...body, displayName: `Renamed ${uniqueSuffix()}` };
    const put = await request.put(`/api/v1/Scopes/${detail.id}`, { data: updated });
    expect(put.status()).toBe(204);

    const got2 = await (await request.get(`/api/v1/Scopes/${detail.id}`)).json();
    expect((got2 as ScopeSummary).displayName).toBe(updated.displayName);
  } finally {
    await request.delete(`/api/v1/Scopes/${detail.id}`);
  }
});

test('scopes.claims sub-resource round-trip', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const claimType = `tests.claim.${uniqueSuffix()}`;
    const post = await request.post(`/api/v1/scopes/${detail.id}/claims`, {
      data: { type: claimType },
    });
    expect([200, 201, 204]).toContain(post.status());

    const list = await request.get(`/api/v1/scopes/${detail.id}/claims`);
    expect(list.status()).toBe(200);
    const claims = (await list.json()) as Array<{ id: number; type: string }>;
    expect(claims.some((c) => c.type === claimType)).toBe(true);
  } finally {
    await request.delete(`/api/v1/Scopes/${detail.id}`);
  }
});
