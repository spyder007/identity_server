import { test, expect } from '../fixtures';
import type { ApiResourceSecretDetail, ApiResourceSummary, SaveApiResource, SaveApiResourceSecret } from './types';
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

// Same hashing/validation bugs applied to API resource secrets as to client
// secrets (both DTOs share Spydersoft.Identity.Admin.Api.Models.SecretValueValidation
// and the same AutoMapper hashing logic) — cover the round-trip and the
// validation rules here too.
test('apiresources.secrets: SharedSecret round-trips through the sub-resource', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const post = await request.post(`/api/v1/apiresources/${detail.id}/secrets`, {
      data: { type: 'SharedSecret', value: `S3cret.${uniqueSuffix()}` } satisfies SaveApiResourceSecret,
    });
    expect(post.status()).toBe(201);
    const created = (await post.json()) as ApiResourceSecretDetail;
    expect(created.type).toBe('SharedSecret');

    const list = await request.get(`/api/v1/apiresources/${detail.id}/secrets`);
    expect(list.status()).toBe(200);
    const secrets = (await list.json()) as ApiResourceSecretDetail[];
    expect(secrets.some((s) => s.id === created.id), 'newly added secret should appear in list').toBe(true);

    const del = await request.delete(`/api/v1/apiresources/${detail.id}/secrets/${created.id}`);
    expect(del.status()).toBe(204);
  } finally {
    await request.delete(`/api/v1/ApiResources/${detail.id}`);
  }
});

test('apiresources.secrets: rejects a malformed X509 thumbprint but accepts a well-formed one', async ({
  request,
}) => {
  const { detail } = await create(request);
  try {
    const bad = await request.post(`/api/v1/apiresources/${detail.id}/secrets`, {
      data: { type: 'X509Thumbprint', value: 'not-a-thumbprint' } satisfies SaveApiResourceSecret,
    });
    expect(bad.status()).toBe(400);

    const good = await request.post(`/api/v1/apiresources/${detail.id}/secrets`, {
      data: { type: 'X509Thumbprint', value: '0'.repeat(40) } satisfies SaveApiResourceSecret,
    });
    expect(good.status()).toBe(201);
  } finally {
    await request.delete(`/api/v1/ApiResources/${detail.id}`);
  }
});

test('apiresources.secrets: rejects a non-certificate X509CertificateBase64 value', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const bad = await request.post(`/api/v1/apiresources/${detail.id}/secrets`, {
      data: { type: 'X509CertificateBase64', value: 'not valid base64!!' } satisfies SaveApiResourceSecret,
    });
    expect(bad.status()).toBe(400);
  } finally {
    await request.delete(`/api/v1/ApiResources/${detail.id}`);
  }
});

test('apiresources.secrets: rejects an unrecognized secret type', async ({ request }) => {
  const { detail } = await create(request);
  try {
    const bad = await request.post(`/api/v1/apiresources/${detail.id}/secrets`, {
      data: { type: 'NotARealSecretType', value: 'whatever' } satisfies SaveApiResourceSecret,
    });
    expect(bad.status()).toBe(400);
  } finally {
    await request.delete(`/api/v1/ApiResources/${detail.id}`);
  }
});
