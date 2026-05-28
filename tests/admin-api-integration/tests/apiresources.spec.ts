import { test, expect } from '../fixtures';
import type { ApiResourceSummary, SaveApiResource } from './types';
import { uniqueSuffix } from './types';

function payload(): SaveApiResource {
  return {
    name: `tests.apires.${uniqueSuffix()}`,
    displayName: `Test API ${uniqueSuffix()}`,
    description: `Created by admin-api-integration tests at ${new Date().toISOString()}`,
    enabled: true,
    nonEditable: false,
  };
}

async function create(request: import('@playwright/test').APIRequestContext) {
  const body = payload();
  const response = await request.post('/api/v1/ApiResources', { data: body });
  expect(response.status(), 'create api resource').toBe(201);
  const detail = (await response.json()) as ApiResourceSummary;
  expect(detail.id).toBeGreaterThan(0);
  return { body, detail };
}

test('apiresources.list returns the seeded admin api resource', async ({ request }) => {
  const response = await request.get('/api/v1/ApiResources');
  expect(response.status()).toBe(200);
  const list = (await response.json()) as ApiResourceSummary[];
  expect(list.some((r) => r.name === 'identity.admin.api')).toBe(true);
});

test('apiresources.create-get-update-delete round-trip', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const got = await (await request.get(`/api/v1/ApiResources/${detail.id}`)).json();
    expect((got as ApiResourceSummary).name).toBe(body.name);

    const updated: SaveApiResource = { ...body, displayName: `Renamed ${uniqueSuffix()}` };
    const put = await request.put(`/api/v1/ApiResources/${detail.id}`, { data: updated });
    expect(put.status()).toBe(204);

    const got2 = await (await request.get(`/api/v1/ApiResources/${detail.id}`)).json();
    expect((got2 as ApiResourceSummary).displayName).toBe(updated.displayName);
  } finally {
    const del = await request.delete(`/api/v1/ApiResources/${detail.id}`);
    expect([204, 404]).toContain(del.status());
  }
});

test('apiresources.scopes sub-resource round-trip', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const scopeName = `tests.scope.${uniqueSuffix()}`;
    const post = await request.post(`/api/v1/apiresources/${detail.id}/scopes`, {
      data: { scope: scopeName },
    });
    expect([200, 201, 204]).toContain(post.status());

    const list = await request.get(`/api/v1/apiresources/${detail.id}/scopes`);
    expect(list.status()).toBe(200);
    const scopes = (await list.json()) as Array<{ id: number; scope: string }>;
    expect(scopes.some((s) => s.scope === scopeName)).toBe(true);
  } finally {
    await request.delete(`/api/v1/ApiResources/${detail.id}`);
  }
});
