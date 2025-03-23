using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Text.Json;
using Windows.Media.Ocr;
using Windows.Globalization;
using System.CommandLine;
using System.CommandLine.Completions;


var fileOption = new Option<string>(
    name: "--file",
    description: "The file to read and display on the console."
) {
    IsRequired = true
};
var languageOption = new Option<string>(
    name: "--language",
    description: "The language that should be used during OCR.",
    getDefaultValue: () => "en-US"
);
var modeOption = new Option<OcrOutputMode>(
    name: "--mode",
    description: "The OCR output mode.",
    getDefaultValue: () => OcrOutputMode.json
);


var rootCommand = new RootCommand("Start an OCR analysis using Windows local OcrEngine")
{
    fileOption,
    languageOption,
    modeOption
};
rootCommand.SetHandler(Handler, fileOption, languageOption, modeOption);

return await rootCommand.InvokeAsync(args);


static async Task Handler(string filepath, string language, OcrOutputMode mode)
{
    var result = await RecognizeAsync(filepath, language);
    var txt = "";

    if (mode == OcrOutputMode.json)
    {
        txt = JsonSerializer.Serialize(result);
    }
    else if (mode == OcrOutputMode.text)
    {
        var sb = new StringBuilder();
        foreach (var l in result.Lines)
        {
            var line = new StringBuilder();

            foreach (var word in l.Words)
            {
                line.Append(word.Text);
                if (!language.Contains("zh"))
                {
                    line.Append(" ");
                }
            }
            sb.Append(line);
            sb.Append(Environment.NewLine);
        }
        txt = sb.ToString();
    }

    Console.WriteLine(txt);
}


static async Task<OcrResult> RecognizeAsync(string filepath, string language)
{
    var path = Path.GetFullPath(filepath);
    var storageFile = await StorageFile.GetFileFromPathAsync(path);
    using var randomAccessStream = await storageFile.OpenReadAsync();
    var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
    using var softwareBitmap = await decoder.GetSoftwareBitmapAsync(
        BitmapPixelFormat.Bgra8,
        BitmapAlphaMode.Premultiplied
    );
    var lang = new Language(language);
    if (OcrEngine.IsLanguageSupported(lang))
    {
        var engine = OcrEngine.TryCreateFromLanguage(lang);
        if (engine != null)
        {
            return await engine.RecognizeAsync(softwareBitmap);
        }
        else
        {
            throw new Exception($"Could not instanciate OcrEngine for language {language}.");
        }
    }
    else
    {
        throw new Exception($"Language {language} is not supported");
    }
}

enum OcrOutputMode
{
    json,
    text
}
