/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as path from "path";
import * as fs from "fs";
import * as semver from "semver";
import * as os from "os";
import { PlatformInformation } from "./../platform";

const MINIMUM_SUPPORTED_DOTNET_CLI: string = "1.0.0";

export class DotNetCliError extends Error {
  public ErrorMessage: string; // the message to display to the user
  public ErrorString: string; // the string to log for this error
}

export class CoreClrDebugUtil {
  private _extensionDir: string = "";
  private _debugAdapterDir: string = "";
  private _installCompleteFilePath: string = "";

  constructor(extensionDir: string) {
    this._extensionDir = extensionDir;
    this._debugAdapterDir = path.join(this._extensionDir, ".debugger");
    this._installCompleteFilePath = path.join(
      this._debugAdapterDir,
      "install.complete"
    );
  }

  public extensionDir(): string {
    if (this._extensionDir === "") {
      throw new Error("Failed to set extension directory");
    }
    return this._extensionDir;
  }

  public debugAdapterDir(): string {
    if (this._debugAdapterDir === "") {
      throw new Error("Failed to set debugadpter directory");
    }
    return this._debugAdapterDir;
  }

  public installCompleteFilePath(): string {
    if (this._installCompleteFilePath === "") {
      throw new Error("Failed to set install complete file path");
    }
    return this._installCompleteFilePath;
  }

  public static async writeEmptyFile(path: string): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      fs.writeFile(path, "", (err) => {
        if (err) {
          reject(err.code);
        } else {
          resolve();
        }
      });
    });
  }

  public defaultDotNetCliErrorMessage(): string {
    return "Failed to find up to date dotnet cli on the path.";
  }

  public static isMacOSSupported(): boolean {
    // .NET Core 2.0 requires macOS 10.12 (Sierra), which is Darwin 16.0+
    // Darwin version chart: https://en.wikipedia.org/wiki/Darwin_(operating_system)
    return semver.gte(os.release(), "16.0.0");
  }

  public static existsSync(path: string): boolean {
    try {
      fs.accessSync(path, fs.constants.F_OK);
      return true;
    } catch (err) {
      if (err.code === "ENOENT" || err.code === "ENOTDIR") {
        return false;
      } else {
        throw Error(err.code);
      }
    }
  }

  public static getPlatformExeExtension(): string {
    if (process.platform === "win32") {
      return ".exe";
    }

    return "";
  }
}

const MINIMUM_SUPPORTED_OSX_ARM64_DOTNET_CLI: string = "6.0.0";

export function getTargetArchitecture(
  platformInfo: PlatformInformation,
  launchJsonTargetArchitecture: string
): string {
  let targetArchitecture = "";

  // On Apple M1 Machines, we need to determine if we need to use the 'x86_64' or 'arm64' debugger.
  if (platformInfo.isMacOS()) {
    // 'targetArchitecture' is specified in launch.json configuration, use that.
    if (launchJsonTargetArchitecture) {
      if (
        launchJsonTargetArchitecture !== "x86_64" &&
        launchJsonTargetArchitecture !== "arm64"
      ) {
        throw new Error(
          `The value '${launchJsonTargetArchitecture}' for 'targetArchitecture' in launch configuraiton is invalid. Expected 'x86_64' or 'arm64'.`
        );
      }
      targetArchitecture = launchJsonTargetArchitecture;
    }

    if (!targetArchitecture) {
      // Unable to retrieve any targetArchitecture, go with platformInfo.
      targetArchitecture = platformInfo.architecture;
    }
  }

  return targetArchitecture;
}
