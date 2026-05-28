import { test, expect } from '../fixtures';
import type { IdentityResourceSummary, SaveIdentityResource } from './types';
import { uniqueSuffix } from './types';

function payload(): SaveIdentityResource {
  return {
    name: `tests.idres.${uniqueSuffix()}`,
    displayName: `Test Identity Resource ${uniqueSuffix()}`,
    description: `Created by admin-api-integration tests at ${new Date().toISOString()}`,
    enabled: true,
    required: false,
    emphasize: false,
    showInDiscoveryDocument: true,
    nonEditable: false,
  };
}

async function create(request: import('@playwright/test').APIRequestContext) {
  const body = payload();
  const response = await request.post('/api/v1/IdentityResources', { data: body });
  expect(response.status(), 'create identity resource').toBe(201);
  const detail = (await response.json()) as IdentityResourceSummary;
  expect(detail.id).toBeGreaterThan(0);
  return { body, detail };
}

test('identityresources.list includes standard scopes', async ({ request }) => {
  const response = await request.get('/api/v1/IdentityResources');
  expect(response.status()).toBe(200);
  const list = (await response.json()) as IdentityResourceSummary[];
  // The seeder always creates openid + profile + email + address + phone + roles
  for (const expected of ['openid', 'profile', 'email']) {
    expect(list.some((r) => r.name === expected)).toBe(true);
  }
});

test('identityresources.create-get-update-delete round-trip', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const got = await (await request.get(`/api/v1/IdentityResources/${detail.id}`)).json();
    expect((got as IdentityResourceSummary).name).toBe(body.name);

    const updated: SaveIdentityResource = { ...body, displayName: `Renamed ${uniqueSuffix()}` };
    const put = await request.put(`/api/v1/IdentityResources/${detail.id}`, { data: updated });
    expect(put.status()).toBe(204);

    const got2 = await (await request.get(`/api/v1/IdentityResources/${detail.id}`)).json();
    expect((got2 as IdentityResourceSummary).displayName).toBe(updated.displayName);
  } finally {
    await request.delete(`/api/v1/IdentityResources/${detail.id}`);
  }
});

test('identityresources.claims sub-resource round-trip', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const claimType = `tests.claim.${uniqueSuffix()}`;
    const post = await request.post(`/api/v1/identityresources/${detail.id}/claims`, {
      data: { type: claimType },
    });
    expect([200, 201, 204]).toContain(post.status());

    const list = await request.get(`/api/v1/identityresources/${detail.id}/claims`);
    expect(list.status()).toBe(200);
    const claims = (await list.json()) as Array<{ id: number; type: string }>;
    expect(claims.some((c) => c.type === claimType)).toBe(true);
  } finally {
    await request.delete(`/api/v1/IdentityResources/${detail.id}`);
  }
});
