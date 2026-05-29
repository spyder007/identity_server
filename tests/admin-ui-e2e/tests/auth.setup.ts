import { test as setup, expect } from '@playwright/test';
import path from 'node:path';

const username = process.env.IDENTITY_ADMIN_USERNAME ?? 'admin';
// Matches the seeded credential in
// Spydersoft.Identity.DataSeeder/Seeding/Identity.cs. Test-only, committed.
const password = process.env.IDENTITY_ADMIN_PASSWORD ?? 'Ch@ng3m3';
// auth.setup.ts lives under tests/; storageState target is .auth/state.json
// at the project root (so it matches playwright.config.ts).
const storageStatePath = path.resolve(__dirname, '..', '.auth', 'state.json');

setup('authenticate via OIDC and persist storage state', async ({ page }) => {
  setup.setTimeout(60_000);

  // The SPA's signed-out screen shows a single "Sign in" button which
  // navigates to /.auth/login (the OidcProxy challenge endpoint).
  await page.goto('/');
  await page.getByRole('button', { name: /sign in/i }).click();

  // Land on the main identity app's login form. asp-for="Username" and
  // asp-for="Password" produce input[name="Username"] / input[name="Password"];
  // the submit is button[type=submit][name=button][value=login].
  await page.waitForURL(/\/Account\/Login/i);
  await page.locator('input[name="Username"]').fill(username);
  await page.locator('input[name="Password"]').fill(password);
  await Promise.all([
    page.waitForURL(/localhost:7050\/?(\?|#|$)/),
    page.locator('button[type="submit"][name="button"][value="login"]').click(),
  ]);

  // The SPA's authenticated shell renders the sidebar with a "Clients" link
  // once /.auth/me returns; if that never appears we're not actually logged in.
  await expect(page.getByRole('link', { name: 'Clients' })).toBeVisible({ timeout: 15_000 });

  await page.context().storageState({ path: storageStatePath });
});
