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
                pattern: '**/*.back',
            }
        ],
        synchronize: {
            // Synchronize the setting section 'languageServerExample' to the server
            configurationSection: 'BacklangLanguageServer',
            fileEvents: workspace.createFileSystemWatcher('**/*.back')
        },
    }

    // Create the language client and start the client.
    const client = new LanguageClient('BacklangLanguageServer', 'Backlang Language Server', serverOptions, clientOptions);
    
    let disposable = client.start();

    

    //context.subscriptions.push(disposable);
}