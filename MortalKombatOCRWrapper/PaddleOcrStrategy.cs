using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.Structure;
using PaddleOCRSharp;

namespace MortalKombatOCRWrapper;

public class PaddleOcrStrategy : IOcrStrategy
{
    private static readonly Lazy<PaddleOCREngine> _engineInstance = new(() =>
    {
        OCRModelConfig config = null; // Adjust as necessary
        var oCRParameter = new OCRParameter();
        return new PaddleOCREngine(config, oCRParameter);
    });

    private static readonly Lazy<List<string>> _mkNames = new(() =>
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileName = "mortal kombat.txt";

        var resourceName = assembly.GetManifestResourceNames()
            .First(str => str.EndsWith(fileName));

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream))
        {
            var names = reader.ReadToEnd();

            // Deserialize the JSON data into CharacterDataMap
            return new List<string>(names.Split("\r\n"));
        }
    });

    public string ExtractTextFromImage(UMat image, Rectangle roi)
    {
        var characterNameRoi = new UMat(image, roi);
        var bitmapImage = characterNameRoi.ToImage<Bgr, byte>().ToBitmap();

        var ocrResult = _engineInstance.Value.DetectText(bitmapImage);

        if (ocrResult.TextBlocks.Count > 0)
        {
            var recognizedText = ocrResult.TextBlocks
                .Where(block => block.Score >= 0.9 && IsEnglish(block.Text))
                .Select(block => block.Text).FirstOrDefault();

            recognizedText = OcrHelper.FindClosestMatch(recognizedText, _mkNames.Value);

            // avoid switching to baraka during pause menu
            if (recognizedText == "BASIC ATTACKS") return string.Empty;

            return recognizedText ?? string.Empty;
        }

        return string.Empty;
    }

    public void Preprocess(UMat thresholded)
    {
        // Logic as mentioned
    }

    private bool IsEnglish(string text)
    {
        return !Regex.IsMatch(text, @"[^\x00-\x7F]+");
    }
}