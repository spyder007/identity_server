/**
 * Gulpfile.js - Task Runner Explorer compatible wrapper
 * Delegates to gulpfile.ts via tsx for gulp v5 + TypeScript support.
 */

const { exec } = require('node:child_process');
const { task, series } = require('gulp');

function runGulpTask(taskName) {
  return function (done) {
    const cmd = `gulp ${taskName} --gulpfile gulpfile.ts`;
    exec(cmd, { cwd: process.cwd(), windowsHide: true }, (error, stdout, stderr) => {
      if (error) {
        console.error(`Error running ${taskName}: ${error.message}`);
        return done(error);
      }
      if (stdout) console.log(stdout);
      if (stderr) console.error(stderr);
      done();
    });
  };
}

task('build', runGulpTask('build'));
task('dev', runGulpTask('dev'));
task('clean', runGulpTask('cleanTask'));
task('cleanTask', runGulpTask('cleanTask'));
task('watch', runGulpTask('watchSass'));
task('watchSass', runGulpTask('watchSass'));
task('css', runGulpTask('css'));
task('tailwind', runGulpTask('tailwind'));
task('assets', runGulpTask('assets'));
task('libraries', runGulpTask('libraries'));

task('develop', series('clean', 'dev'));
task('production', series('clean', 'build'));
task('default', series('build'));
