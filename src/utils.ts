import { fileURLToPath } from "node:url";
import { join, dirname } from "node:path";

export const __dirname = dirname(fileURLToPath(import.meta.url));

export const EXE_PATH = "bin/net481/win-x86/windows_media_ocr_cli.exe";

export const DEFAULT_MODULE_ROOT = join(__dirname, "..");
