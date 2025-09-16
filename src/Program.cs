using System;
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

var rootCommand = new RootCommand("Start an OCR analysis using Windows local OcrEngine.")
{
    fileOption,
    languageOption
};
rootCommand.SetHandler(async (files, lang) => await Handler(files, lang), fileOption, languageOption);

return await rootCommand.InvokeAsync(args);

static async Task Handler(string[] filepaths, string language)
{
    var results = await RecognizeBatchFromPath(filepaths, language);
    var txt = JsonSerializer.Serialize(results);
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

class CustomOcrResult
{
    public string language { get; set; }
    public OcrResult result { get; set; }
}