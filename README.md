# Node Windows OCR

A command-line tool based on the Windows OCR engine for recognizing text in images. This tool uses Windows' built-in OCR functionality and supports multiple languages.

## Features

- Supports text recognition in various image formats
- Supports multiple languages (based on Windows supported languages)
- Supports batch processing of multiple image files
- Provides Node.js interface for easy integration into JavaScript/TypeScript projects

## Installation

```bash
npm install node-windows-ocr
```

## Usage

```ts
import { recognizeBatchFromPath } from "node-windows-ocr";

const results = await recognizeBatchFromPath(["image1.png"]);
console.log(results);
```

## Supported Languages

This tool supports all languages supported by the Windows OCR engine, including but not limited to:

- English (en-US)
- Simplified Chinese (zh-Hans)
- Traditional Chinese (zh-Hant)
- Japanese (ja-JP)
- Korean (ko-KR)
- German (de-DE)
- French (fr-FR)
- Spanish (es-ES)
- And more...

## Development

### Requirements

- Windows 10 or higher
- .NET Framework 4.8.1 or higher
- Node.js 18 or higher

### Build

```bash
# Install dependencies
pnpm install

# Build project
pnpm build
```

### Test

```bash
pnpm test
```

## License

MIT
