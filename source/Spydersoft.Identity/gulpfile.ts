import { src, watch, dest, series } from "gulp";
// Use require for better compatibility with current dependencies
const gulpSass = require('gulp-sass');
const gulpRename = require('gulp-rename');
const gulpCleanCSS = require('gulp-clean-css');
const dartSass = require('sass');
const gulpPostcss = require('gulp-postcss');
import { deleteAsync } from 'del';

// Configure sass to use the Dart Sass compiler
const sassCompiler = gulpSass(dartSass);

/**
 * Interface for library copy job configuration
 */
interface LibraryJob {
  src: string;
  dest: string;
}

/**
 * Interface for library configuration
 */
interface LibraryConfig {
  lib: string;
  jobs: LibraryJob[];
}

/**
 * Build paths configuration
 */
const paths = {
  dist: "wwwroot/",
  src: "./",
  styles: "Styles/",
  scripts: "Scripts/",
  node_modules: "node_modules/",
  libDest: "lib/",
  assets: "Assets/",
} as const;

/**
 * Configuration for libraries to copy from node_modules
 * Clean configuration with only required dependencies for Tailwind + DaisyUI
 */
const libsToCopy: LibraryConfig[] = [
  {
    lib: "jquery",
    jobs: [
      {
        src: "/dist/**/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
  {
    lib: "jquery-validation-unobtrusive",
    jobs: [
      {
        src: "/dist/**/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
  {
    lib: "jquery-validation",
    jobs: [
      {
        src: "/dist/**/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
  {
    lib: "@fortawesome/fontawesome-free",
    jobs: [
      {
        src: "/css/**/*",
        dest: "{libDest}/font-awesome/css",
      },
      {
        src: "/webfonts/**/*",
        dest: "{libDest}/font-awesome/webfonts",
      },
    ],
  },
  {
    lib: "davidshimjs-qrcodejs",
    jobs: [
      {
        src: "/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
  {
    lib: "moment",
    jobs: [
      {
        src: "/min/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
];

/**
 * Converts SCSS files to CSS with Tailwind processing and creates minified versions
 * Returns a stream to properly handle async completion
 */
function convertSass(): NodeJS.ReadWriteStream {
  return src(paths.styles + "scss/style.scss")
    .pipe(sassCompiler({
      includePaths: [
        './Styles/scss',
        './node_modules'
      ],
      silenceDeprecations: ['legacy-js-api']
    }).on('error', sassCompiler.logError))
    .pipe(gulpPostcss([
      require('tailwindcss'),
      require('autoprefixer')
    ]))
    .pipe(dest(paths.dist + "css"))
    .pipe(gulpCleanCSS({ 
      compatibility: "*",
      level: 2 
    }))
    .pipe(gulpRename({ suffix: ".min" }))
    .pipe(dest(paths.dist + "css"));
}

/**
 * Processes Tailwind CSS directly for faster development builds
 */
function processTailwind(): NodeJS.ReadWriteStream {
  return src(paths.styles + "scss/style.scss")
    .pipe(sassCompiler({
      includePaths: [
        './Styles/scss',
        './node_modules'
      ],
      silenceDeprecations: ['legacy-js-api']
    }).on('error', sassCompiler.logError))
    .pipe(gulpPostcss([
      require('tailwindcss'),
      require('autoprefixer')
    ]))
    .pipe(dest(paths.dist + "css"));
}

/**
 * Watches SCSS files for changes and recompiles with Tailwind
 */
function sassWatch(): void {
  watch([
    paths.styles + "scss/**/*.scss",
    "./Views/**/*.cshtml",
    "./Components/**/*.razor",
    "./Pages/**/*.cshtml",
    "./tailwind.config.js"
  ], series(processTailwind));
}

/**
 * Copies assets from Assets directory to wwwroot
 * Returns a stream to properly handle async completion
 */
function copyAssets(): NodeJS.ReadWriteStream {
  return src(paths.assets + "**/*")
    .pipe(dest(paths.dist));
}

/**
 * Copies all configured libraries from node_modules to wwwroot/lib
 * Uses async/await pattern for proper completion handling
 */
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

/**
 * Copies a specific library from node_modules to destination
 * Returns a Promise to properly handle async completion
 * @param libName - Name of the library
 * @param srcPath - Source path within the library
 * @param destPath - Destination path (with placeholders)
 */
function copyLibrary(libName: string, srcPath: string, destPath: string): Promise<void> {
  return new Promise((resolve, reject) => {
    const fullSource: string = paths.node_modules + libName + srcPath;

    let fullDest: string = destPath
      .replace("{libDest}", paths.libDest)
      .replace("{libName}", libName);
    fullDest = paths.dist + fullDest;

    console.log(`Copying ${fullSource} to ${fullDest}`);
    
    src(fullSource)
      .pipe(dest(fullDest))
      .on('end', () => resolve())
      .on('error', (error) => reject(error));
  });
}

/**
 * Clean task to remove generated files
 */
async function clean(): Promise<void> {
  try {
    await deleteAsync([
      paths.dist + 'css/**',
      paths.dist + 'lib/**'
    ]);
    console.log('Cleaned output directories');
  } catch (error) {
    console.error('Error cleaning directories:', error);
    throw error;
  }
}

/**
 * Task to build all SCSS files with Tailwind processing
 */
export const sass = convertSass;

/**
 * Task for fast Tailwind development builds
 */
export const tailwind = processTailwind;

/**
 * Task to watch SCSS files for changes
 */
export const watchSass = sassWatch;

/**
 * Task to copy assets
 */
export const assets = copyAssets;

/**
 * Task to copy libraries
 */
export const libraries = copyLibraries;

/**
 * Task to clean output directories
 */
export const cleanTask = clean;

/**
 * Development task with watching and fast Tailwind builds
 */
export const dev = series(processTailwind, copyAssets, copyLibraries, watchSass);

/**
 * Production build task with full optimization
 */
export const build = series(convertSass, copyAssets, copyLibraries);

/**
 * Default export
 */
export default build;