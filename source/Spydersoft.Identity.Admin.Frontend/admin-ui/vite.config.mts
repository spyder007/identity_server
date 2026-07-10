import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";
import fs from "node:fs";
import path from "node:path";
import child_process from "node:child_process";

const isTest = process.env.NODE_ENV === "test" || process.env.VITEST === "true";
const isBuild =
  process.env.NODE_ENV === "production" || process.argv.includes("build");
// VITE_DEV_HTTP=1 → run the dev server over plain HTTP and proxy API/auth
// calls to the BFF on http://localhost:7040 instead of https://localhost:7041.
// Used by the Playwright e2e suite (CI) where Kestrel's HTTPS dev-cert pickup
// is unreliable from a bash-spawned dotnet host.
const devHttp = process.env.VITE_DEV_HTTP === "1";
const bffTarget = devHttp ? "http://localhost:7040/" : "https://localhost:7041/";

let keyFilePath = "";
let certFilePath = "";

if (!isTest && !isBuild && !devHttp) {
  const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ""
      ? `${process.env.APPDATA}/ASP.NET/https`
      : `${process.env.HOME}/.aspnet/https`;

  if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
  }

  const certificateArg = process.argv
    .map((arg) => arg.match(/--name=(?<value>.+)/i))
    .find(Boolean);
  const certificateName = certificateArg
    ? certificateArg.groups!.value
    : "admin-ui";

  certFilePath = path.join(baseFolder, `${certificateName}.pem`);
  keyFilePath = path.join(baseFolder, `${certificateName}.key`);

  if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (
      0 !==
      child_process.spawnSync(
        "dotnet",
        [
          "dev-certs",
          "https",
          "--export-path",
          certFilePath,
          "--format",
          "Pem",
          "--no-password",
        ],
        { stdio: "inherit" },
      ).status
    ) {
      throw new Error("Could not create certificate.");
    }
  }
}

export default defineConfig({
  plugins: [react()],
  build: {
    chunkSizeWarningLimit: 1200,
  },
  server: {
    proxy: {
      "^/api": {
        target: bffTarget,
        secure: false,
      },
      "^/.auth": {
        target: bffTarget,
        secure: false,
      },
      "^/.diag": {
        target: bffTarget,
        secure: false,
      },
      "^/livez": {
        target: bffTarget,
        secure: false,
      },
      "^/readyz": {
        target: bffTarget,
        secure: false,
      },
    },
    port: parseInt(process.env.PORT ?? "7050"),
    cors: {
      origin: "*",
    },
    ...(!isTest &&
      !isBuild &&
      !devHttp &&
      keyFilePath &&
      certFilePath && {
        https: {
          key: fs.readFileSync(keyFilePath),
          cert: fs.readFileSync(certFilePath),
        },
      }),
  },
  test: {
    globals: true,
    environment: "jsdom",
    setupFiles: "./tests/setup.js",
    reporters: ["html", "junit"],
    outputFile: "./output/test/junit.xml",
    coverage: {
      provider: "v8",
      reporter: ["html", "cobertura", "lcov", "text"],
      reportsDirectory: "./output/coverage",
      exclude: [
        "**/node_modules/**",
        "**/tests/**",
        "**/dist/**",
        "**/output/**",
        "**/vite.config.mts",
        "**/eslint.config.js",
        "**/coverage/**",
        "**/.yarn/**",
        "**/src/api/**",
        "**/*.d.ts",
      ],
    },
  },
});
