/**
 * Gulpfile.js - Task Runner Explorer compatible version
 * This file provides Visual Studio Task Runner Explorer integration
 * The actual implementation is in gulpfile.ts
 */

const { exec } = require('child_process');
const { task, series } = require('gulp');

// Helper function to run tasks using ts-node directly with the TypeScript gulpfile
function runTsGulpTask(taskName) {
  return function(done) {
    const command = `npx ts-node -e "require('./gulpfile.ts').${taskName}?.()" 2>/dev/null || npx gulp ${taskName} --require ts-node/register --gulpfile gulpfile.ts`;
    exec(command, { 
      cwd: process.cwd(),
      windowsHide: true 
    }, (error, stdout, stderr) => {
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

// Helper function to run npm scripts
function runNpmScript(scriptName) {
  return function(done) {
    exec(`npm run ${scriptName}`, (error, stdout, stderr) => {
      if (error) {
        console.error(`Error: ${error}`);
        return done(error);
      }
      console.log(stdout);
      if (stderr) console.error(stderr);
      done();
    });
  };
}

// Simple direct gulp task invocation
function runDirectGulpTask(taskName) {
  return function(done) {
    exec(`npx gulp ${taskName} --require ts-node/register --gulpfile gulpfile.ts`, (error, stdout, stderr) => {
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

// Define main workflow tasks
task('build', runDirectGulpTask('build'));
task('dev', runDirectGulpTask('dev'));
task('clean', runDirectGulpTask('cleanTask'));
task('watch', runDirectGulpTask('watchSass'));

// Individual tasks
task('sass', runDirectGulpTask('sass'));
task('tailwind', runDirectGulpTask('tailwind'));
task('assets', runDirectGulpTask('assets'));
task('libraries', runDirectGulpTask('libraries'));

// Workflow combinations
task('develop', series('clean', 'dev'));
task('production', series('clean', 'build'));

// Default task
task('default', series('build'));

// Additional helper tasks for compatibility
task('cleanTask', runDirectGulpTask('cleanTask'));
task('watchSass', runDirectGulpTask('watchSass'));