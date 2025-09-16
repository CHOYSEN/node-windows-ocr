using System;
using System.Text;
using Windows.Graphics.Imaging;
using Windows.Storage;
using System.Text.Json;
using Windows.Media.Ocr;
using Windows.Globalization;
using System.CommandLine;

var fileOption = new Option<string[]>(
    name: "--files",
    description: "The files to read and display on the console."
)
{
    IsRequired = true,
    AllowMultipleArgumentsPerToken = true
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


var rootCommand = new RootCommand("Start an OCR analysis using Windows local OcrEngine.")
{
    fileOption,
    languageOption,
    modeOption
};
rootCommand.SetHandler(async (files, lang, mode) => await Handler(files, lang, mode), fileOption, languageOption, modeOption);

return await rootCommand.InvokeAsync(args);

static async Task Handler(string[] filepaths, string language, OcrOutputMode mode)
{
    var results = await RecognizeBatchFromPath(filepaths, language);
    var txt = "";

    if (mode == OcrOutputMode.json)
    {
        txt = JsonSerializer.Serialize(results);
    }
    else if (mode == OcrOutputMode.text)
    {
        var sb = new StringBuilder();
        foreach (var result in results)
        {
            foreach (var l in result.result.Lines)
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
        }
        txt = sb.ToString();
    }

    Console.WriteLine(txt);
}

static async Task<List<CustomOcrResult>> RecognizeBatchFromPath(string[] filepaths, string language)
{
    var engine = IsLanguageSupported(language) ? OcrEngine.TryCreateFromLanguage(new Language(language)) : OcrEngine.TryCreateFromUserProfileLanguages();
    if (engine == null) { throw new Exception($"Could not instanciate OcrEngine for language {language}."); }

    var results = new List<CustomOcrResult>();
    foreach (var filepath in filepaths)
    {
        var path = Path.GetFullPath(filepath);
        var storageFile = await StorageFile.GetFileFromPathAsync(path);
        using var randomAccessStream = await storageFile.OpenReadAsync();
        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
        using var softwareBitmap = await decoder.GetSoftwareBitmapAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied
        );

        var result = new CustomOcrResult
        {
            language = engine.RecognizerLanguage.LanguageTag,
            result = await engine.RecognizeAsync(softwareBitmap)
        };

        results.Add(result);
    }

    return results;
}

static bool IsLanguageSupported(string language)
{
    return OcrEngine.IsLanguageSupported(new Language(language));
}


enum OcrOutputMode
{
    json,
    text
}

class CustomOcrResult
{
    public string language { get; set; }
    public OcrResult result { get; set; }
}