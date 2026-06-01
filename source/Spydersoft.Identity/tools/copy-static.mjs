/**
 * Copies static assets and vendored client libraries into wwwroot.
 * Replaces the old gulp `copyAssets` + `copyLibraries` tasks.
 *   - Assets/**            -> wwwroot/**
 *   - selected node_modules -> wwwroot/lib/**
 */
import { cp, mkdir } from "node:fs/promises";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const root = join(dirname(fileURLToPath(import.meta.url)), "..");
const wwwroot = join(root, "wwwroot");
const nodeModules = join(root, "node_modules");

// node_modules subpath -> wwwroot-relative destination
const libs = [
  ["@fortawesome/fontawesome-free/css", "lib/font-awesome/css"],
  ["@fortawesome/fontawesome-free/webfonts", "lib/font-awesome/webfonts"],
  ["aspnet-client-validation/dist", "lib/aspnet-client-validation"],
  ["davidshimjs-qrcodejs", "lib/davidshimjs-qrcodejs"],
  ["moment/min", "lib/moment"],
];

async function run() {
  // 1. Static assets
  await cp(join(root, "Assets"), wwwroot, { recursive: true });
  console.log("Copied Assets -> wwwroot");

  // 2. Vendored libraries
  for (const [src, dest] of libs) {
    const from = join(nodeModules, src);
    const to = join(wwwroot, dest);
    await mkdir(to, { recursive: true });
    await cp(from, to, { recursive: true });
    console.log(`Copied ${src} -> wwwroot/${dest}`);
  }
}

run().catch((err) => {
  console.error(err);
  process.exit(1);
});
