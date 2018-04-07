/* eslint-disable */

var cpx = require('cpx');
var replace = require('replace-in-file');
var find = require('find-in-files');
var mv = require('mv');
var rmdir = require('rmdir');
var mkdirp = require('mkdirp');
var gentlyCopy = require('gently-copy');

var appfileList = [
    './src/app/app-routing.module.ts',
    './src/app/app.component.css',
    './src/app/app.component.html',
    './src/app/app.component.ts',
    './src/app/app.module.ts'];

var rootfileList = [
    './src/tsconfig.app.json',
    './src/empty_config.json',
    './src/config.json',
    './src/index.html',
    './src/styles.alt.css',
    './src/styles.css',
    './src/fonts',
    './src/assets'];

    var scriptfileList = [
        './postinstall.js',
    ];

var distdest = './dist';
var rootdest = './dist/temp/';
var appdest = './dist/temp/app/';

mkdirp.sync(appdest);

gentlyCopy(scriptfileList, distdest, { overwrite: true });
gentlyCopy(appfileList, appdest, { overwrite: true });
gentlyCopy(rootfileList, rootdest, { overwrite: true });

mv("./dist/temp/empty_config.json", "./dist/temp/config.json", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });

// to update imports to use npm module
find.findSync("name", ".", "package.json").then(s => {

    try {
        var nameLine = s["package.json"].line[0];
        var nameSplit = nameLine.split('"');
        var name = nameSplit[3];

        var options1 = {
            files: [
                "./dist/temp/app/app-routing.module.ts",
                "./dist/temp/app/app.module.ts",
                "./dist/temp/app/app.component.ts"
            ],
            from: [/\.\/.*\/.*\.component/g, /\.\/.*\.(service|directive|handler)/g, /\.\/route-data/g],
            to: name
        };

        replace.sync(options1);

    } catch (e) {
        console.error('Error occurred updating name:', e);
    }
});
