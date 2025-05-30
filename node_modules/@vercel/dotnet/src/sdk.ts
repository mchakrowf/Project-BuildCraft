
import axios from 'axios';
import shell from 'shelljs';
import fs from 'fs/promises';
import path from 'path';
export async function InstallNET(workdir: string, channel = "current"){
    var dotNetInstaller = path.join(workdir, "dotnet.sh");
    var dotNetSDK = path.join(workdir, ".net");
    var execTarget = path.join(dotNetSDK, "dotnet");
    if((await fs.stat(dotNetSDK)).isDirectory()){
        return execTarget;
    }
    if(!(await fs.stat(dotNetInstaller)).isFile()){
        var install = await axios.get("https://dot.net/v1/dotnet-install.sh")
        await fs.writeFile(dotNetInstaller, install.data);
        await fs.chmod(dotNetInstaller, 0o755);
    }
    shell.exec(`bash ${dotNetInstaller} -Channel ${channel} -InstallDir .net`, {cwd: workdir});
    if((await fs.stat(execTarget)).isFile()){
        return execTarget;
    }
    return null;
}