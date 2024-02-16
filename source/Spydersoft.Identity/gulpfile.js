const { src, watch, dest, series } = require("gulp");
const sass = require("gulp-sass")(require('sass'));
const rename = require("gulp-rename");
const cleanCSS = require("gulp-clean-css");

const paths = {
  dist: "wwwroot/",
  src: "./",
  styles: "Styles/",
  scripts: "Scripts/",
  node_modules: "node_modules/",
  libDest: "lib/",
  assets: "Assets/",
};

const libsToCopy = [
  {
    lib: "bootstrap",
    jobs: [
      {
        src: "/dist/**/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
  {
    lib: "popper.js",
    jobs: [
      {
        src: "/dist/**/*",
        dest: "{libDest}{libName}",
      },
    ],
  },
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
      {
        src: "/svgs/**/*",
        dest: "{libDest}/font-awesome/svgs",
      },
      {
        src: "/sprites/**/*",
        dest: "{libDest}/font-awesome/sprites",
      },
    ],
  },
  {
    lib: "simple-line-icons",
    jobs: [
      {
        src: "/css/**/*",
        dest: "{libDest}{libName}/css",
      },
      {
        src: "/fonts/**/*",
        dest: "{libDest}{libName}/fonts",
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

function convertSass(cb) {
  /// <summary>
  /// </summary>
  return src(paths.styles + "/scss/style.scss")
    .pipe(sass())
    .pipe(dest(paths.dist + "css"))
    .pipe(cleanCSS({ compatibility: "ie8" }))
    .pipe(rename({ suffix: ".min" }))
    .pipe(dest(paths.dist + "css"));
}

function sassWatch() {
  /// <summary>
  /// </summary>
  watch(paths.styles + "scss/**/*.scss", ["sass"]);
}

function copyAssets() {
  /// <summary>
  /// </summary>
  return src(paths.assets + "**/*").pipe(dest(paths.dist));
}

function copyLibraries(cb) {
  /// <summary>
  /// </summary>
  /// <param name="cb">The cb.</param>
  libsToCopy.forEach(function (lib) {
    /// <summary>
    /// </summary>
    /// <param name="lib">The library.</param>
    console.log("Copying " + lib.lib);

    lib.jobs.forEach(function (job) {
      /// <summary>
      /// </summary>
      /// <param name="job">The job.</param>
      copyLibrary(src, dest, lib.lib, job.src, job.dest);
    });
  });
  cb();
}

function copyLibrary(gulpSrc, gulpDest, libName, src, dest) {
  /// <summary>
  /// Copies the library.
  /// </summary>
  /// <param name="libName">Name of the library.</param>
  /// <param name="src">The source.</param>
  /// <param name="dest">The dest.</param>
  let fullSource = paths.node_modules + libName + src;

  let fullDest = dest
    .replace("{libDest}", paths.libDest)
    .replace("{libName}", libName);
  fullDest = paths.dist + fullDest;

  console.log("Copying " + fullSource + " to " + fullDest);
  gulpSrc(fullSource).pipe(gulpDest(fullDest));
}

exports.sassWatch = sassWatch;
exports.default = series(convertSass, copyAssets, copyLibraries);
