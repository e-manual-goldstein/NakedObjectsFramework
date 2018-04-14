/* eslint-disable */

var gentlyCopy = require('gently-copy');
var fs = require('fs');

var appfileList = [
    './temp/app/auth.interceptor.ts',
    './temp/app/app-routing.module.ts',
    './temp/app/app.component.css',
    './temp/app/app.component.html',
    './temp/app/app.component.ts',
    './temp/app/app.module.ts'];

var rootfileList = [
    './temp/tsconfig.app.json',
    './temp/config.json',
    './temp/index.html',
    './temp/styles.alt.css',
    './temp/styles.css',
    './temp/fonts',
    './temp/assets'];

var rootdest = '../../src';
var appdest = '../../src/app/';

gentlyCopy(appfileList, appdest, { overwrite: true });
gentlyCopy(rootfileList, rootdest, { overwrite: true });

var ngCliFile = '../../.angular-cli.json';

if (fs.existsSync(ngCliFile)) {
    var f = fs.readFileSync(ngCliFile, 'utf8');
    var ac = JSON.parse(f);
    var assets = ac.apps[0].assets;
    var found = false;

    for (var i = 0; i < assets.length; i++) {
        found = assets[i] === "config.json" || found;
    }

    if (!found) {
        assets.push("config.json");
        fs.writeFile(ngCliFile, JSON.stringify(ac, null, 2));
    }
}
