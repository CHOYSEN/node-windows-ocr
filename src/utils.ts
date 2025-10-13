import { fileURLToPath } from "node:url";
import { join, dirname } from "node:path";
import { createRequire } from "node:module";

export const require = createRequire(import.meta.url);

export const electron = (() => {
  try {
    return require("electron") as typeof import("electron");
  } catch (e) {}
})();

export const isElectron = !!electron;

export const __dirname = dirname(fileURLToPath(import.meta.url));

const exe = "bin/net481/win-x86/windows_media_ocr_cli.exe";
export const bin =
  !isElectron || !electron!.app.isPackaged
    ? join(__dirname, "..", exe)
    : join(
        process.resourcesPath,
        "app.asar.unpacked",
        "node_modules/node-windows-ocr",
        exe
      );
