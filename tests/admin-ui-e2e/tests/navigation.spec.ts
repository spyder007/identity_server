import { test, expect, type Page } from '@playwright/test';

test.beforeEach(async ({ page }) => {
  await page.goto('/');
});

async function navigateTo(page: Page, label: string) {
  await page.getByRole('link', { name: label, exact: true }).click();
}

test('sidebar lists every section under the correct group', async ({ page }) => {
  for (const label of [
    'Clients',
    'API Resources',
    'Identity Resources',
    'Scopes',
    'Users',
    'Roles',
  ]) {
    await expect(page.getByRole('link', { name: label, exact: true })).toBeVisible();
  }
});

test('Clients page lists the seeded admin-frontend client', async ({ page }) => {
  await navigateTo(page, 'Clients');
  await expect(page.getByRole('heading', { name: 'Clients' })).toBeVisible();
  // identity.admin.frontend is always seeded; identity.admin.tests is too (we
  // depend on it for the API suite) — both should render in the DataTable.
  await expect(page.getByText('identity.admin.frontend')).toBeVisible();
  await expect(page.getByText('identity.admin.tests')).toBeVisible();
});

test('API Resources page lists identity.admin.api', async ({ page }) => {
  await navigateTo(page, 'API Resources');
  await expect(page.getByRole('heading', { name: 'API Resources' })).toBeVisible();
  await expect(page.getByText('identity.admin.api')).toBeVisible();
});

test('Identity Resources page lists the standard scopes', async ({ page }) => {
  await navigateTo(page, 'Identity Resources');
  await expect(page.getByRole('heading', { name: 'Identity Resources' })).toBeVisible();
  for (const expected of ['openid', 'profile', 'email']) {
    await expect(page.getByText(expected, { exact: true })).toBeVisible();
  }
});

test('Scopes page lists the seeded admin scopes', async ({ page }) => {
  await navigateTo(page, 'Scopes');
  await expect(page.getByRole('heading', { name: 'Scopes' })).toBeVisible();
  await expect(page.getByText('identity:admin:read')).toBeVisible();
  await expect(page.getByText('identity:admin:write')).toBeVisible();
});

test('Users page lists the seeded admin user', async ({ page }) => {
  await navigateTo(page, 'Users');
  await expect(page.getByRole('heading', { name: 'Users' })).toBeVisible();
  await expect(page.getByText('admin@localhost.net')).toBeVisible();
});

test('Roles page renders (may be empty until a role is seeded)', async ({ page }) => {
  await navigateTo(page, 'Roles');
  await expect(page.getByRole('heading', { name: 'Roles' })).toBeVisible();
});
