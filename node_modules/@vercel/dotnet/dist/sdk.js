"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.InstallNET = void 0;
const tslib_1 = require("tslib");
const axios_1 = (0, tslib_1.__importDefault)(require("axios"));
const shelljs_1 = (0, tslib_1.__importDefault)(require("shelljs"));
const promises_1 = (0, tslib_1.__importDefault)(require("fs/promises"));
const path_1 = (0, tslib_1.__importDefault)(require("path"));
function InstallNET(workdir, channel = "current") {
    return (0, tslib_1.__awaiter)(this, void 0, void 0, function* () {
        var dotNetInstaller = path_1.default.join(workdir, "dotnet.sh");
        var dotNetSDK = path_1.default.join(workdir, ".net");
        var execTarget = path_1.default.join(dotNetSDK, "dotnet");
        if ((yield promises_1.default.stat(dotNetSDK)).isDirectory()) {
            return execTarget;
        }
        if (!(yield promises_1.default.stat(dotNetInstaller)).isFile()) {
            var install = yield axios_1.default.get("https://dot.net/v1/dotnet-install.sh");
            yield promises_1.default.writeFile(dotNetInstaller, install.data);
            yield promises_1.default.chmod(dotNetInstaller, 0o755);
        }
        shelljs_1.default.exec(`bash ${dotNetInstaller} -Channel ${channel} -InstallDir .net`, { cwd: workdir });
        if ((yield promises_1.default.stat(execTarget)).isFile()) {
            return execTarget;
        }
        return null;
    });
}
exports.InstallNET = InstallNET;
