import { defineConfig, devices } from '@playwright/test';
import path from 'node:path';

const appHostProject = path.resolve(__dirname, '../../source/Spydersoft.Identity.AppHost');
const adminUiDir = path.resolve(__dirname, '../../source/Spydersoft.Identity.Admin.Frontend/admin-ui');

// Under the Testing launch profile we run the whole BFF/Vite/SPA chain over
// plain HTTP — Kestrel's HTTPS dev-cert pickup is unreliable from a
// bash-spawned dotnet host. The Vite dev server proxies /api, /.auth,
// /livez, /readyz to the BFF on http://localhost:7040; the BFF handles OIDC
// against identity on http://localhost:7020 and proxies /api to admin-api
// on http://localhost:7030.
const spaBaseUrl = process.env.IDENTITY_ADMIN_UI_BASE_URL ?? 'http://localhost:7050';
const bffLivez = 'http://localhost:7040/livez';

const storageStatePath = path.resolve(__dirname, '.auth', 'state.json');

export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: 'html',
  use: {
    baseURL: spaBaseUrl,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    {
      name: 'setup',
      testMatch: /auth\.setup\.ts/,
    },
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        storageState: storageStatePath,
      },
      dependencies: ['setup'],
    },
  ],
  webServer: [
    // Aspire stack: Postgres + seeder + identity + admin-api + BFF, all over
    // HTTP under the Testing profile. We wait on the BFF's /livez probe so
    // every downstream is up before tests start.
    {
      command: `dotnet run --project "${appHostProject}" --launch-profile Testing`,
      url: bffLivez,
      timeout: 240_000,
      reuseExistingServer: !process.env.CI,
      stdout: 'pipe',
      stderr: 'pipe',
    },
    // Vite dev server in HTTP mode (VITE_DEV_HTTP=1 makes the config skip
    // dev-cert export and proxy to http://localhost:7040 instead of the
    // default HTTPS targets).
    {
      command: 'yarn dev',
      cwd: adminUiDir,
      url: spaBaseUrl,
      timeout: 60_000,
      reuseExistingServer: !process.env.CI,
      env: { VITE_DEV_HTTP: '1' },
    },
  ],
});
