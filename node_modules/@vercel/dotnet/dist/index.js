"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.shouldServe = exports.prepareCache = exports.build = exports.version = void 0;
const tslib_1 = require("tslib");
const path_1 = (0, tslib_1.__importDefault)(require("path"));
const build_utils_1 = require("@vercel/build-utils");
Object.defineProperty(exports, "shouldServe", { enumerable: true, get: function () { return build_utils_1.shouldServe; } });
const sdk_1 = require("./sdk");
const shelljs_1 = (0, tslib_1.__importDefault)(require("shelljs"));
exports.version = 3;
function build({ files, entrypoint, workPath, config = {}, meta = {}, }) {
    return (0, tslib_1.__awaiter)(this, void 0, void 0, function* () {
        // Check if now dev mode is used
        if (meta.isDev) {
            console.log(`
      🐘 vercel dev is not supported right now.
      Please use dotnet built-in development server.

      
    `);
            process.exit(255);
        }
        console.log('🐘 Downloading user files');
        // Collect user provided files
        const userFiles = yield (0, build_utils_1.download)(files, workPath, meta);
        console.log('🐘 Downloading .NET build SDK');
        // Collect runtime files containing .NET bins and libs
        var dotnet = yield (0, sdk_1.InstallNET)(workPath);
        var buildPath = path_1.default.join(workPath, "bin");
        console.log('🐘 Adding .NET Lambda Support');
        shelljs_1.default.exec(`${dotnet} add package Amazon.Lambda.RuntimeSupport`, { cwd: workPath });
        console.log('🐘 Building .NET project');
        shelljs_1.default.exec(`${dotnet} publish -sc -c Release -o ${buildPath}`, { cwd: workPath });
        // Collect user files, files creating during build (composer vendor)
        // and other files and prefix them with "user" (/var/task/user folder).
        const harverstedFiles = (0, build_utils_1.rename)(yield (0, build_utils_1.glob)('**', {
            /**cwd: workPath,
            ignore:
              config && config.excludeFiles
                ? Array.isArray(config.excludeFiles) ? config.excludeFiles : [config.excludeFiles]
                : ['node_modules/**', 'now.json', '.nowignore', 'vercel.json', '.vercelignore', '.net', 'dotnet.sh'], */
            cwd: buildPath
        }), name => path_1.default.join('user', name));
        // Show some debug notes during build
        if (process.env.NOW_PHP_DEBUG === '1') {
            console.log('🐘 Entrypoint:', entrypoint);
            console.log('🐘 Config:', config);
            console.log('🐘 Work path:', workPath);
            console.log('🐘 Meta:', meta);
            console.log('🐘 User files:', Object.keys(harverstedFiles));
            console.log('🐘 .NET:', dotnet);
        }
        console.log('🐘 Creating lambda');
        const lambda = yield (0, build_utils_1.createLambda)({
            files: Object.assign({}, harverstedFiles),
            runtime: 'provided.al2',
            handler: 'no-needed',
            environment: {
                NOW_ENTRYPOINT: entrypoint,
                NOW_DEV: meta.isDev ? '1' : '0'
            },
        });
        return { output: lambda };
    });
}
exports.build = build;
;
function prepareCache({ workPath }) {
    return (0, tslib_1.__awaiter)(this, void 0, void 0, function* () {
        return Object.assign({}, (yield (0, build_utils_1.glob)('.net/**', workPath)));
    });
}
exports.prepareCache = prepareCache;
