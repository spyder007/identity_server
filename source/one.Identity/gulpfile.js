﻿//gulpfile.js

var gulp = require('gulp');
var sass = require('gulp-sass');
var runSequence = require('run-sequence');

//style paths
var stylePath = "Styles/";
var destination = "wwwroot";

var sassFiles = 'Styles/**/*.scss',
    cssDest = 'wwwroot/css/';

gulp.task('sass', function () {
    gulp.src(stylePath + 'scss/style.scss', {base: 'Styles/scss'})
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(cssDest));
});

gulp.task('copy:css', function () {
    gulp.src(stylePath + 'css/**/*')
        .pipe(gulp.dest(destination + '/css'));
});

gulp.task('copy:img', function () {
    return gulp.src(stylePath + 'img/**/*')
        .pipe(gulp.dest(destination + '/img'));
});

gulp.task('copy:fonts', function () {
    return gulp.src(stylePath + 'fonts/**/*')
        .pipe(gulp.dest(destination + '/fonts'));
});

gulp.task('copy:style-js', function () {
    return gulp.src(stylePath + 'js/**/*')
        .pipe(gulp.dest(destination + '/js'));
});

gulp.task('copy:js', function () {
    return gulp.src('Scripts/**/*.js')
        .pipe(gulp.dest(destination + '/js'));
});


gulp.task('build:dist', function (callback) {
    runSequence('sass', 'copy:css', 'copy:img', 'copy:fonts', 'copy:js', 'copy:style-js', callback);
});
