using System.Drawing;
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
        File.ReadAllLines("tessdata/mortal kombat.txt").ToList());

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