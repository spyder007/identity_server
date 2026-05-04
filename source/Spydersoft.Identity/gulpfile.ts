import { src, watch, dest, series } from "gulp";
import through2 from "through2";
import Vinyl from "vinyl";
import { promises as fsp } from "node:fs";
import { join } from "node:path";

// These packages are CommonJS; require() is available in ts-node CJS mode
/* eslint-disable @typescript-eslint/no-require-imports */
const gulpRename = require("gulp-rename");
const gulpPostcss = require("gulp-postcss");
const lightningcss = require("lightningcss");
/* eslint-enable @typescript-eslint/no-require-imports */

interface LibraryJob {
  src: string;
  dest: string;
}

interface LibraryConfig {
  lib: string;
  jobs: LibraryJob[];
}

const paths = {
  dist: "wwwroot/",
  src: "./",
  styles: "Styles/",
  scripts: "Scripts/",
  node_modules: "node_modules/",
  libDest: "lib/",
  assets: "Assets/",
} as const;

const libsToCopy: LibraryConfig[] = [
  {
    lib: "jquery",
    jobs: [{ src: "/dist/**/*", dest: "{libDest}{libName}" }],
  },
  {
    lib: "jquery-validation-unobtrusive",
    jobs: [{ src: "/dist/**/*", dest: "{libDest}{libName}" }],
  },
  {
    lib: "jquery-validation",
    jobs: [{ src: "/dist/**/*", dest: "{libDest}{libName}" }],
  },
  {
    lib: "@fortawesome/fontawesome-free",
    jobs: [
      { src: "/css/**/*", dest: "{libDest}/font-awesome/css" },
      { src: "/webfonts/**/*", dest: "{libDest}/font-awesome/webfonts" },
    ],
  },
  {
    lib: "davidshimjs-qrcodejs",
    jobs: [{ src: "/*", dest: "{libDest}{libName}" }],
  },
  {
    lib: "moment",
    jobs: [{ src: "/min/*", dest: "{libDest}{libName}" }],
  },
];

/** Minify CSS using lightningcss (supports modern CSS: OKLCH, nesting, layers) */
function minifyWithLightningcss() {
  return through2.obj(function (file: Vinyl, _enc: string, cb: (err?: Error | null, data?: Vinyl) => void) {
    if (file.isNull() || !file.contents) {
      return cb(null, file);
    }

    try {
      // Vinyl contents is Buffer or Stream; only Buffer is safe to pass directly
      const input: Buffer = file.isBuffer()
        ? file.contents
        : Buffer.concat([]);

      const result = lightningcss.transform({
        filename: file.basename || "style.css",
        code: input,
        minify: true,
        sourceMap: false,
      });

      file.contents = Buffer.from(result.code);
      cb(null, file);
    } catch (err) {
      cb(err instanceof Error ? err : new Error(String(err)));
    }
  });
}

function processCSS(): NodeJS.ReadWriteStream {
  return src(paths.styles + "style.css")
    .pipe(
      gulpPostcss([
        require("@tailwindcss/postcss"),
        require("autoprefixer")(),
      ])
    )
    .pipe(dest(paths.dist + "css"))
    .pipe(minifyWithLightningcss())
    .pipe(gulpRename({ suffix: ".min" }))
    .pipe(dest(paths.dist + "css"));
}

function processCSSFast(): NodeJS.ReadWriteStream {
  return src(paths.styles + "style.css")
    .pipe(
      gulpPostcss([
        require("@tailwindcss/postcss"),
        require("autoprefixer")(),
      ])
    )
    .pipe(dest(paths.dist + "css"));
}

function cssWatch(): void {
  watch(
    [
      paths.styles + "**/*.css",
      "./Views/**/*.cshtml",
      "./Components/**/*.razor",
      "./Pages/**/*.cshtml",
      "./Scripts/**/*.js",
    ],
    series(processCSSFast)
  );
}

function copyAssets(): NodeJS.ReadWriteStream {
  return src(paths.assets + "**/*").pipe(dest(paths.dist));
}

async function copyLibraries(): Promise<void> {
  const copyPromises: Promise<void>[] = [];

  libsToCopy.forEach((lib: LibraryConfig) => {
    console.log(`Copying ${lib.lib}`);
    lib.jobs.forEach((job: LibraryJob) => {
      copyPromises.push(copyLibrary(lib.lib, job.src, job.dest));
    });
  });

  await Promise.all(copyPromises);
}

async function copyLibrary(
  libName: string,
  srcPath: string,
  destPath: string
): Promise<void> {
  // Strip glob suffix to get the source directory:
  // "/dist/**/*" → "/dist", "/min/*" → "/min", "/*" → ""
  const dirPart = srcPath.replace(/\/\*.*$/, "");
  const fullSource = join(paths.node_modules, libName + dirPart);

  let fullDest = destPath
    .replace("{libDest}", paths.libDest)
    .replace("{libName}", libName);
  fullDest = join(paths.dist, fullDest);

  console.log(`Copying ${fullSource} to ${fullDest}`);
  await fsp.mkdir(fullDest, { recursive: true });
  await fsp.cp(fullSource, fullDest, { recursive: true });
}

async function clean(): Promise<void> {
  // del v8 is ESM-only; dynamic import() works from CJS context
  const { deleteAsync } = await import("del");
  try {
    await deleteAsync([paths.dist + "css/**", paths.dist + "lib/**"]);
    console.log("Cleaned output directories");
  } catch (error) {
    console.error("Error cleaning directories:", error);
    throw error;
  }
}

export const css = processCSS;
export const tailwind = processCSSFast;
export const watchSass = cssWatch;
export const assets = copyAssets;
export const libraries = copyLibraries;
export const cleanTask = clean;

export const dev = series(processCSSFast, copyAssets, copyLibraries, cssWatch);
export const build = series(processCSS, copyAssets, copyLibraries);

export default build;
