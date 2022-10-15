"use strict";

import * as path from "path";
import { PlatformInformation } from "./platform";
import * as coreclrdebug from "./coreclr-debug/activate";
import { workspace, ExtensionContext } from "vscode";
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
  TransportKind,
} from "vscode-languageclient/node";

export async function activate(context: ExtensionContext) {
  let platformInfo: PlatformInformation;
  try {
    platformInfo = await PlatformInformation.GetCurrent();
  } catch (error) {
    throw error;
  }

  let serverExe = "dotnet";

  let server = context.asAbsolutePath(path.join("bin", "LSP-Server.dll"));

  let serverOptions: ServerOptions = {
    run: { command: serverExe, args: [server] },
    debug: { command: serverExe, args: [server] },
  };

  // Options to control the language client
  let clientOptions: LanguageClientOptions = {
    documentSelector: [
      {
        pattern: "**/*.back",
      },
    ],

    synchronize: {
      configurationSection: "BacklangLanguageServer",
      fileEvents: workspace.createFileSystemWatcher("**/*.back"),
    },
  };

  let coreClrDebugPromise = Promise.resolve();

  // activate coreclr-debug
  coreClrDebugPromise = coreclrdebug.activate(context, platformInfo);

  const client = new LanguageClient(
    "BacklangLanguageServer",
    "Backlang Language Server",
    serverOptions,
    clientOptions
  );

  client.start();
}
