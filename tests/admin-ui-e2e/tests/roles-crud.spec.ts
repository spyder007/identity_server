import { test, expect } from '@playwright/test';

function uniqueRoleName(): string {
  return `e2e.role.${crypto.randomUUID().replaceAll('-', '').slice(0, 8)}`;
}

test('roles: create then delete via the UI', async ({ page }) => {
  const roleName = uniqueRoleName();

  await page.goto('/');
  await page.getByRole('link', { name: 'Roles', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Roles' })).toBeVisible();

  // Create
  await page.getByRole('link', { name: /new role/i }).click();
  await expect(page.getByRole('heading', { name: /create role/i })).toBeVisible();
  // The Name field's label isn't wired with htmlFor/id, so getByLabel
  // doesn't match. Use the placeholder — also distinct on this page.
  await page.getByPlaceholder('administrator').fill(roleName);
  await page.getByRole('button', { name: /create role/i }).click();

  // RoleEdit calls navigate(`/roles/${id}`, { replace: true }) after the POST
  // resolves. Wait for that URL transition so we know the row exists in the
  // DB before we navigate to the list.
  await page.waitForURL(/\/roles\/[0-9a-f-]+$/i);

  await page.getByRole('link', { name: /back/i }).click();
  await expect(page.getByRole('heading', { name: 'Roles' })).toBeVisible();
  const newRow = page.getByRole('row').filter({ hasText: roleName });
  await expect(newRow).toBeVisible();

  // Delete (use the row's delete button; PrimeReact ConfirmDialog accepts via
  // the dialog's "Delete" button — scope to the dialog to disambiguate.)
  await newRow.getByRole('button', { name: 'Delete' }).click();
  const confirm = page.getByRole('dialog');
  await expect(confirm).toBeVisible();
  await confirm.getByRole('button', { name: 'Delete' }).click();

  await expect(page.getByRole('row').filter({ hasText: roleName })).toHaveCount(0);
});
