/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as path from "path";
import * as vscode from "vscode";
import * as common from "./../common";
import { CoreClrDebugUtil, getTargetArchitecture } from "./util";
import { PlatformInformation } from "./../platform";
import { DotnetDebugConfigurationProvider } from "./debugConfigurationProvider";

let _debugUtil: CoreClrDebugUtil = null;

export async function activate(
  context: vscode.ExtensionContext,
  platformInformation: PlatformInformation
) {
  _debugUtil = new CoreClrDebugUtil(context.extensionPath);

  const factory = new DebugAdapterExecutableFactory();

  context.subscriptions.push(
    vscode.debug.registerDebugConfigurationProvider(
      "coreclr",
      new DotnetDebugConfigurationProvider(platformInformation)
    )
  );
  context.subscriptions.push(
    vscode.debug.registerDebugConfigurationProvider(
      "clr",
      new DotnetDebugConfigurationProvider(platformInformation)
    )
  );
  context.subscriptions.push(
    vscode.debug.registerDebugAdapterDescriptorFactory("coreclr", factory)
  );
  context.subscriptions.push(
    vscode.debug.registerDebugAdapterDescriptorFactory("clr", factory)
  );
}

// The activate method registers this factory to provide DebugAdapterDescriptors
// If the debugger components have not finished downloading, the proxy displays an error message to the user
// Else it will launch the debug adapter
export class DebugAdapterExecutableFactory
  implements vscode.DebugAdapterDescriptorFactory
{
  async createDebugAdapterDescriptor(
    _session: vscode.DebugSession,
    executable: vscode.DebugAdapterExecutable | undefined
  ): Promise<vscode.DebugAdapterDescriptor> {
    let util = new CoreClrDebugUtil(common.getExtensionPath());

    // use the executable specified in the package.json if it exists or determine it based on some other information (e.g. the session)

    const command = path.join(
      common.getExtensionPath(),
      ".debugger",
      "x32",
      "vsdbg-ui" + CoreClrDebugUtil.getPlatformExeExtension()
    );
    executable = new vscode.DebugAdapterExecutable(command, [], {
      env: {
        DOTNET_ROOT: "",
      },
    });

    // make VS Code launch the DA executable
    return executable;
  }
}
