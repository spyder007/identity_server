import { defineConfig } from '@playwright/test';
import path from 'node:path';

const appHostProject = path.resolve(__dirname, '../../source/Spydersoft.Identity.AppHost');

// Admin.Api endpoint — both ports are pinned in the AppHost; the Testing
// launch profile mirrors http (the AppHost itself listens on 7000, but each
// hosted resource keeps its own pinned port).
const adminApiBaseUrl = process.env.IDENTITY_ADMIN_API_BASE_URL ?? 'http://localhost:7030';

export default defineConfig({
  testDir: './tests',
  // Tests share a single backing database; run serial to avoid cross-test
  // contention on the same resource ids.
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: 'html',
  globalSetup: require.resolve('./globalSetup'),
  use: {
    baseURL: adminApiBaseUrl,
  },
  webServer: {
    command: `dotnet run --project "${appHostProject}" --launch-profile Testing`,
    // Wait on the Admin.Api OpenAPI doc — proves identity + seeder + admin-api
    // are all up and the JWT validation pipeline is alive.
    url: `${adminApiBaseUrl}/openapi/v1.json`,
    timeout: 240_000,
    reuseExistingServer: !process.env.CI,
    stdout: 'pipe',
    stderr: 'pipe',
  },
});
