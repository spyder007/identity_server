import { test, expect } from '../fixtures';
import type { RoleClaim, RoleSummary, SaveRole } from './types';
import { uniqueSuffix } from './types';

function payload(): SaveRole {
  return { name: `tests.role.${uniqueSuffix()}` };
}

async function create(request: import('@playwright/test').APIRequestContext) {
  const body = payload();
  const response = await request.post('/api/v1/Roles', { data: body });
  expect(response.status(), 'create role').toBe(201);
  const detail = (await response.json()) as RoleSummary;
  expect(typeof detail.id).toBe('string');
  expect(detail.id.length).toBeGreaterThan(0);
  return { body, detail };
}

test('roles.list endpoint returns an array', async ({ request }) => {
  const response = await request.get('/api/v1/Roles');
  expect(response.status()).toBe(200);
  const list = (await response.json()) as RoleSummary[];
  expect(Array.isArray(list)).toBe(true);
});

test('roles.create-get-update-delete round-trip', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const got = (await (await request.get(`/api/v1/Roles/${detail.id}`)).json()) as RoleSummary;
    expect(got.name).toBe(body.name);

    const renamed: SaveRole = { name: `${body.name}.renamed` };
    const put = await request.put(`/api/v1/Roles/${detail.id}`, { data: renamed });
    expect(put.status()).toBe(204);

    const got2 = (await (await request.get(`/api/v1/Roles/${detail.id}`)).json()) as RoleSummary;
    expect(got2.name).toBe(renamed.name);
  } finally {
    const del = await request.delete(`/api/v1/Roles/${detail.id}`);
    expect([204, 404]).toContain(del.status());
  }
});

test('roles.claims add, list, and remove by claim type', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const claimType = `tests.claim.${uniqueSuffix()}`;
    const claimValue = `value-${uniqueSuffix()}`;

    const post = await request.post(`/api/v1/Roles/${detail.id}/claims`, {
      data: { type: claimType, value: claimValue },
    });
    expect(post.status()).toBe(204);

    const list = (await (await request.get(`/api/v1/Roles/${detail.id}/claims`)).json()) as RoleClaim[];
    const row = list.find((c) => c.type === claimType);
    expect(row).toBeDefined();
    expect(row?.value).toBe(claimValue);

    const remove = await request.delete(
      `/api/v1/Roles/${detail.id}/claims/${encodeURIComponent(claimType)}`,
    );
    expect(remove.status()).toBe(204);

    const listAfter = (await (await request.get(`/api/v1/Roles/${detail.id}/claims`)).json()) as RoleClaim[];
    expect(listAfter.some((c) => c.type === claimType)).toBe(false);
  } finally {
    await request.delete(`/api/v1/Roles/${detail.id}`);
  }
});

test('roles.delete returns 404 after deletion', async ({ request }) => {
  const { detail } = await create(request);
  const del = await request.delete(`/api/v1/Roles/${detail.id}`);
  expect(del.status()).toBe(204);
  const got = await request.get(`/api/v1/Roles/${detail.id}`);
  expect(got.status()).toBe(404);
});
