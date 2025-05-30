import path from 'path';
import {
  createLambda,
  rename,
  shouldServe,
  BuildOptions,
  PrepareCacheOptions,
  glob,
  download,
  Lambda
} from '@vercel/build-utils';
import { InstallNET } from './sdk';
import shell from 'shelljs';

export const version = 3;

export async function build({
  files,
  entrypoint,
  workPath,
  config = {},
  meta = {},
}: BuildOptions): Promise<{ output: Lambda }> {
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
  const userFiles: RuntimeFiles = await download(files, workPath, meta);

  console.log('🐘 Downloading .NET build SDK');

  // Collect runtime files containing .NET bins and libs
  var dotnet = await InstallNET(workPath);
  var buildPath = path.join(workPath, "bin");
  
  console.log('🐘 Adding .NET Lambda Support');
  shell.exec(`${dotnet} add package Amazon.Lambda.RuntimeSupport`, {cwd: workPath});

  console.log('🐘 Building .NET project');
  shell.exec(`${dotnet} publish -sc -c Release -o ${buildPath} -p:PublishReadyToRun=true`, {cwd: workPath});

  // Collect user files, files creating during build (composer vendor)
  // and other files and prefix them with "user" (/var/task/user folder).
  const harverstedFiles = rename(
    await glob('**', {
      /**cwd: workPath,
      ignore:
        config && config.excludeFiles
          ? Array.isArray(config.excludeFiles) ? config.excludeFiles : [config.excludeFiles]
          : ['node_modules/**', 'now.json', '.nowignore', 'vercel.json', '.vercelignore', '.net', 'dotnet.sh'], */
          cwd: buildPath
    }),
    name => path.join('user', name)
  );

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

  const lambda = await createLambda({
    files: {
      // Located at /var/task/user
      ...harverstedFiles,
      // Located at /var/task/php (php bins + ini + modules)
      // Located at /var/task/lib (shared libs)
    },
    runtime: 'provided.al2',
    handler: 'no-needed',
    environment: {
      NOW_ENTRYPOINT: entrypoint,
      NOW_DEV: meta.isDev ? '1' : '0'
    },
  });

  return { output: lambda };
};

export async function prepareCache({ workPath }: PrepareCacheOptions): Promise<RuntimeFiles> {
  return {
    // Runtime
    ...(await glob('.net/**', workPath)),

  };
}

export { shouldServe };