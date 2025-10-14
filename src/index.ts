import spawn from "nano-spawn";
import { join } from "node:path";
import { EXE_PATH, DEFAULT_MODULE_ROOT } from "./utils";

export interface OCRResult {
  Language: string;
  Result: {
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
  {
    language,
    moduleRoot = DEFAULT_MODULE_ROOT,
  }: { language?: string; moduleRoot?: string } = {}
) {
  const bin = join(moduleRoot, EXE_PATH);
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
