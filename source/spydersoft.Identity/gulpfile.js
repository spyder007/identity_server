/// <binding AfterBuild='build:dist' />
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass')(require('sass'));
var autoprefixer = require('gulp-autoprefixer');
var cleanCSS = require('gulp-clean-css');
var rename = require('gulp-rename');

gulp.paths = {
    dist: 'wwwroot/',
    src: './',
    styles: 'Styles/',
    scripts: 'Scripts/',
    node_modules: "node_modules/",
    libDest: 'lib/',
    assets: 'Assets/'
};

gulp.libsToCopy = [
    {
        lib: "bootstrap",
        jobs: [
            {
                src: "/dist/**/*",
                dest: "{libDest}{libName}"
            }
        ]
    },
    {
        lib: "popper.js",
        jobs: [
            {
                src: "/dist/**/*",
                dest: "{libDest}{libName}"
            }
        ]
    },
    {
        lib: "jquery",
        jobs: [
            {
                src: "/dist/**/*",
                dest: "{libDest}{libName}"
            }
        ]
    },
    {
        lib: "jquery-validation-unobtrusive",
        jobs: [
            {
                src: "/dist/**/*",
                dest: "{libDest}{libName}"
            }
        ]
    },
    {
        lib: "jquery-validation",
        jobs: [
            {
                src: "/dist/**/*",
                dest: "{libDest}{libName}"
            }
        ]
    },
    {
        lib: "@fortawesome/fontawesome-free",
        jobs: [
            {
                src: "/css/**/*",
                dest: "{libDest}/font-awesome/css"
            },
            {
                src: "/webfonts/**/*",
                dest: "{libDest}/font-awesome/webfonts"
            },
            {
                src: "/svgs/**/*",
                dest: "{libDest}/font-awesome/svgs"
            },
            {
                src: "/sprites/**/*",
                dest: "{libDest}/font-awesome/sprites"
            }
        ]
    },
    {
        lib: "simple-line-icons",
        jobs: [
            {
                src: "/css/**/*",
                dest: "{libDest}{libName}/css"
            },
            {
                src: "/fonts/**/*",
                dest: "{libDest}{libName}/fonts"
            }
        ]
    },
    {
        lib: "davidshimjs-qrcodejs",
        jobs: [
            {
                src: "/*",
                dest: "{libDest}{libName}"
            }
        ]
    }
];


var paths = gulp.paths;

gulp.pkg = require('./package.json');
var pkg = gulp.pkg;

gulp.task('sass', function () {
    return gulp.src(paths.styles + '/scss/style.scss')
        .pipe(sass())
        .pipe(autoprefixer())
        .pipe(gulp.dest(paths.dist + 'css'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.dist + 'css'));
});

gulp.task('sass:watch', function () {
    gulp.watch(paths.styles + 'scss/**/*.scss', ['sass']);
});

gulp.task('copy:assets', function () {
    return gulp.src(paths.assets + '**/*')
        .pipe(gulp.dest(paths.dist));
});


gulp.task('copy:libraries',
    function (cb) {
        gulp.libsToCopy.forEach(function (lib) {
            console.log("Copying " + lib.lib);

            lib.jobs.forEach(function (job) {
                copyLibrary(lib.lib, job.src, job.dest);
            });
        });
        cb();
    });

function copyLibrary(libName, src, dest) {

    var fullSource = paths.node_modules + libName + src;

    var fullDest = dest.replace("{libDest}", paths.libDest).replace("{libName}", libName);
    fullDest = paths.dist + fullDest;

    console.log("Copying " + fullSource + " to " + fullDest);
    gulp.src(fullSource)
        .pipe(gulp.dest(fullDest));
}



gulp.task('build:dist', gulp.series('sass', 'copy:assets', "copy:libraries"));

