import { test, expect, type Page } from '@playwright/test';

function uniqueClientId(): string {
  return `e2e.client.${crypto.randomUUID().replaceAll('-', '').slice(0, 8)}`;
}

// PrimeReact's Dropdown renders both a visually-hidden native <select> (for
// accessibility/forms) and a visible ".p-dropdown-label" span with the same
// text, so a plain getByText() match is ambiguous. Scope to the visible
// label's class instead.
async function selectSecretType(page: Page, currentLabel: string, nextLabel: string) {
  await page.locator('.p-dropdown-label', { hasText: currentLabel }).click();
  await page.getByRole('option', { name: nextLabel }).click();
}

// Regression coverage for the Secrets panel rework: Type is now a dropdown
// (not free text), Value switches to a textarea with type-specific guidance
// for the X509 certificate type, Expires is a date/time picker, and
// obviously-malformed X509 values are rejected client-side before ever
// reaching the API.
test('client secrets: type dropdown, expiration picker, and X509 value validation', async ({ page }) => {
  const clientId = uniqueClientId();

  await page.goto('/');
  await page.getByRole('link', { name: 'Clients', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Clients' })).toBeVisible();

  // Create a throwaway client — ClientEdit only shows the Secrets tab once
  // the client exists (isNew becomes false after the POST resolves).
  await page.getByRole('link', { name: /new client/i }).click();
  await expect(page.getByRole('heading', { name: /create client/i })).toBeVisible();
  await page.getByPlaceholder('unique-client-id').fill(clientId);
  await page.getByPlaceholder('My Application').fill(`E2E Client ${clientId}`);
  await page.getByRole('button', { name: /create client/i }).click();
  await page.waitForURL(/\/clients\/\d+$/);

  await page.getByRole('tab', { name: 'Secrets' }).click();
  await expect(page.getByRole('heading', { name: 'Client secrets' })).toBeVisible();

  // 1. Default type (SharedSecret) round-trips, including setting an
  //    expiration via the date/time picker's "Now" shortcut (showButtonBar
  //    swaps in "Now" instead of "Today" because showTime is enabled).
  await page.getByRole('button', { name: 'Choose Date' }).click();
  await page.getByRole('button', { name: 'Now' }).click();
  await page.getByPlaceholder('Plaintext secret value').fill(`s3cret-${clientId}`);
  await page.getByRole('button', { name: 'Add secret' }).click();

  const sharedRow = page.getByRole('row').filter({ hasText: 'SharedSecret' });
  await expect(sharedRow).toBeVisible();
  await expect(sharedRow).toContainText(new Date().getFullYear().toString());

  // 2. Switching to the X509 thumbprint type updates the placeholder and
  //    rejects an obviously malformed value before it reaches the API.
  await selectSecretType(page, 'Shared secret', 'X509 certificate thumbprint');
  await page.getByPlaceholder(/SHA-1 thumbprint/).fill('not-a-thumbprint');
  await page.getByRole('button', { name: 'Add secret' }).click();
  await expect(page.getByText(/Thumbprint must be exactly 40 hexadecimal characters/)).toBeVisible();
  await expect(page.getByRole('row').filter({ hasText: 'X509Thumbprint' })).toHaveCount(0);

  // A well-formed (40 hex char) thumbprint is accepted.
  await page.getByPlaceholder(/SHA-1 thumbprint/).fill('0'.repeat(40));
  await page.getByRole('button', { name: 'Add secret' }).click();
  await expect(page.getByRole('row').filter({ hasText: 'X509Thumbprint' })).toBeVisible();

  // 3. The base64 certificate type switches Value to a multi-line textarea.
  //    (The dropdown resets to "Shared secret" after each successful add —
  //    onCreated() resets the whole create-draft, not just the value.)
  await selectSecretType(page, 'Shared secret', 'X509 certificate (base64)');
  const valueField = page.getByPlaceholder(/Base64-encoded DER certificate/);
  await expect(valueField).toBeVisible();
  await expect(await valueField.evaluate((el) => el.tagName)).toBe('TEXTAREA');

  // A non-base64 value is rejected before it reaches the API.
  await valueField.fill('not valid base64!!');
  await page.getByRole('button', { name: 'Add secret' }).click();
  await expect(page.getByText(/Value must be valid Base64/)).toBeVisible();
  await expect(page.getByRole('row').filter({ hasText: 'X509CertificateBase64' })).toHaveCount(0);

  // Cleanup: delete the client via the list page.
  await page.getByRole('link', { name: /back/i }).click();
  await expect(page.getByRole('heading', { name: 'Clients' })).toBeVisible();
  const clientRow = page.getByRole('row').filter({ hasText: clientId });
  await clientRow.getByRole('button', { name: 'Delete' }).click();
  const confirm = page.getByRole('dialog');
  await expect(confirm).toBeVisible();
  await confirm.getByRole('button', { name: 'Delete' }).click();
  await expect(page.getByRole('row').filter({ hasText: clientId })).toHaveCount(0);
});
