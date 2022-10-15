/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as vscode from "vscode";

import { PlatformInformation } from "../platform";

export class DotnetDebugConfigurationProvider
  implements vscode.DebugConfigurationProvider
{
  constructor(public platformInformation: PlatformInformation) {}

  public async resolveDebugConfigurationWithSubstitutedVariables(
    folder: vscode.WorkspaceFolder | undefined,
    debugConfiguration: vscode.DebugConfiguration,
    token?: vscode.CancellationToken
  ): Promise<vscode.DebugConfiguration> {
    if (!debugConfiguration) {
      return null;
    }

    return debugConfiguration;
  }
}
