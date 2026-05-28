import { test as base, expect, type APIRequestContext } from '@playwright/test';
import { readFileSync } from 'node:fs';
import path from 'node:path';

const tokenFile = path.join(__dirname, '.auth', 'token.json');

function loadToken(): string {
  try {
    const { token } = JSON.parse(readFileSync(tokenFile, 'utf-8')) as { token?: string };
    if (!token) throw new Error('token field missing');
    return token;
  } catch (e) {
    throw new Error(
      `Could not read admin token from ${tokenFile}. globalSetup should write it before tests run. (${(e as Error).message})`,
    );
  }
}

/**
 * Replaces the default `request` fixture with one pre-configured with the
 * Bearer token from globalSetup. Tests should import `test` / `expect` from
 * this module and accept `{ request }` as usual.
 */
export const test = base.extend<{ request: APIRequestContext }>({
  request: async ({ playwright }, use, testInfo) => {
    const baseURL = testInfo.project.use.baseURL;
    const ctx = await playwright.request.newContext({
      baseURL,
      extraHTTPHeaders: { Authorization: `Bearer ${loadToken()}` },
    });
    await use(ctx);
    await ctx.dispose();
  },
});

export { expect };
