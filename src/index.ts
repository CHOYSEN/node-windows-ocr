import spawn from "nano-spawn";
import { fileURLToPath } from "node:url";
import { join, dirname } from "node:path";

const __dirname = dirname(fileURLToPath(import.meta.url));
export const bin = join(
  __dirname,
  "../bin/net481/win-x86/windows_media_ocr_cli.exe"
);

export interface OCRResult {
  language: string;
  result: {
    Lines: {
      Text: string;
      Words: {
        BoundingRect: {
          X: number;
          Y: number;
          Width: number;
          Height: number;
          Left: number;
          Top: number;
          Right: number;
          Bottom: number;
          IsEmpty: boolean;
        };
        Text: string;
      }[];
    }[];
    Text: string;
    TextAngle: number | null;
  };
}

export async function recognizeBatchFromPath(
  files: string[],
  { language }: { language?: string } = {}
) {
  const res = await spawn(bin, [
    ...(language ? ["--language", language] : []),
    "--files",
    files.join(" "),
  ]);

  if (res.stderr) {
    throw new Error(res.stderr);
  } else {
    return JSON.parse(res.stdout) as OCRResult[];
  }
}
