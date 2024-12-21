'use strict';

import * as path from 'path';
import { workspace, ExtensionContext } from 'vscode';
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
  TransportKind
} from 'vscode-languageclient/node';

export function activate(context: ExtensionContext) {

    let serverExe = "dotnet";

    let server = context.asAbsolutePath(
      path.join("bin", "LSP-Server.dll")
    )

    let serverOptions: ServerOptions = {
        run: { command: serverExe, args: [server] },
        debug: { command: serverExe, args: [server] }
    }

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
      documentSelector: [
        {
          pattern: "**/*.sc",
        },
      ],

      synchronize: {
        configurationSection: "SocordiaLanguageServer",
        fileEvents: workspace.createFileSystemWatcher("**/*.sc"),
      },
    };

    const client = new LanguageClient(
      "SocordiaLanguageServer",
      "Socordia Language Server",
      serverOptions,
      clientOptions
    );
    
    client.start();
}