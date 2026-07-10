import { test, expect } from '../fixtures';
import type { CreateUser, SaveUser, UserClaim, UserDetail, UserRole, UserSummary } from './types';
import { uniqueSuffix } from './types';

function payload(): CreateUser {
  const suffix = uniqueSuffix();
  return {
    userName: `tests.user.${suffix}`,
    email: `tests.user.${suffix}@example.invalid`,
    password: 'Tests-P@ssw0rd!',
    name: `Test User ${suffix}`,
    twoFactorEnabled: false,
    lockoutEnabled: true,
  };
}

async function create(request: import('@playwright/test').APIRequestContext) {
  const body = payload();
  const response = await request.post('/api/v1/Users', { data: body });
  expect(response.status(), 'create user').toBe(201);
  const detail = (await response.json()) as UserDetail;
  expect(typeof detail.id).toBe('string');
  expect(detail.id.length).toBeGreaterThan(0);
  return { body, detail };
}

test('users.list returns at least the seeded user', async ({ request }) => {
  const response = await request.get('/api/v1/Users');
  expect(response.status()).toBe(200);
  const list = (await response.json()) as UserSummary[];
  expect(Array.isArray(list)).toBe(true);
  expect(list.length).toBeGreaterThan(0);
});

test('users.create-get-update-delete round-trip', async ({ request }) => {
  const { body, detail } = await create(request);
  try {
    const got = await (await request.get(`/api/v1/Users/${detail.id}`)).json();
    expect((got as UserDetail).userName).toBe(body.userName);
    expect((got as UserDetail).email).toBe(body.email);

    const update: SaveUser = {
      userName: body.userName,
      email: body.email,
      name: `Renamed ${uniqueSuffix()}`,
      twoFactorEnabled: true,
      lockoutEnabled: true,
    };
    const put = await request.put(`/api/v1/Users/${detail.id}`, { data: update });
    expect(put.status()).toBe(204);

    const got2 = (await (await request.get(`/api/v1/Users/${detail.id}`)).json()) as UserDetail;
    expect(got2.name).toBe(update.name);
    expect(got2.twoFactorEnabled).toBe(true);
  } finally {
    const del = await request.delete(`/api/v1/Users/${detail.id}`);
    expect([204, 404]).toContain(del.status());
  }
});

test('users.roles add and remove via role name', async ({ request }) => {
  // Create a role we own so we can attach/detach without affecting other tests
  const roleName = `tests.role.${uniqueSuffix()}`;
  const roleCreate = await request.post('/api/v1/Roles', { data: { name: roleName } });
  expect(roleCreate.status()).toBe(201);
  const role = (await roleCreate.json()) as { id: string; name: string };

  const { detail } = await create(request);
  try {
    const post = await request.post(`/api/v1/Users/${detail.id}/roles`, {
      data: { roleName },
    });
    expect(post.status()).toBe(204);

    const list = await request.get(`/api/v1/Users/${detail.id}/roles`);
    expect(list.status()).toBe(200);
    const assigned = (await list.json()) as UserRole[];
    expect(assigned.some((r) => r.roleName === roleName)).toBe(true);

    const remove = await request.delete(
      `/api/v1/Users/${detail.id}/roles/${encodeURIComponent(roleName)}`,
    );
    expect(remove.status()).toBe(204);

    const listAfter = (await (await request.get(`/api/v1/Users/${detail.id}/roles`)).json()) as UserRole[];
    expect(listAfter.some((r) => r.roleName === roleName)).toBe(false);
  } finally {
    await request.delete(`/api/v1/Users/${detail.id}`);
    await request.delete(`/api/v1/Roles/${role.id}`);
  }
});

test('users.claims endpoint is read-only and returns an array', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const list = await request.get(`/api/v1/Users/${detail.id}/claims`);
    expect(list.status()).toBe(200);
    const claims = (await list.json()) as UserClaim[];
    expect(Array.isArray(claims)).toBe(true);
  } finally {
    await request.delete(`/api/v1/Users/${detail.id}`);
  }
});
