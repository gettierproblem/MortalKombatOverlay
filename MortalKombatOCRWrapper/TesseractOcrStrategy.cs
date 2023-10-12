using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
//using Tesseract;

namespace MortalKombatOCRWrapper;

public class TesseractOcrStrategy : IOcrStrategy
{
    private readonly List<string> _mkNames;

    public TesseractOcrStrategy()
    {
        _mkNames = File.ReadAllLines("tessdata/mortal kombat.txt").ToList();
    }

    public string ExtractTextFromImage(UMat image, Rectangle roi)
    {
        var characterNameRoi = new UMat(image, roi);
        var bitmapImage = characterNameRoi.ToImage<Bgr, byte>().ToBitmap();

        var recognizedText = "";

        //using (var ocrEngine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
        //{
        //    ocrEngine.DefaultPageSegMode = PageSegMode.SingleBlock;
        //    ocrEngine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ- ");
        //    ocrEngine.SetVariable("tessedit_load_sublangs", "eng.user-words");
        //    ocrEngine.SetVariable("user_words_suffix", "tessdata/mortal kombat.txt");

        //    using var img = PixConverter.ToPix(bitmapImage);
        //    using var page = ocrEngine.Process(img);
        //    recognizedText = page.GetText().Trim();
        //    recognizedText = OcrHelper.FindClosestMatch(recognizedText, _mkNames);
        //}

        return recognizedText;
    }

    public void Preprocess(UMat thresholded)
    {
        CvInvoke.CvtColor(thresholded, thresholded, ColorConversion.Bgr2Gray);
        CvInvoke.Resize(thresholded, thresholded, new Size(thresholded.Cols * 2, thresholded.Rows * 2), 0, 0,
            Inter.Lanczos4);
        CvInvoke.Threshold(thresholded, thresholded, 230, 255, ThresholdType.BinaryInv);

        var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
        CvInvoke.Erode(thresholded, thresholded, kernel, new Point(-1, -1), 1, BorderType.Reflect, default);
    }

   // private static Rectangle ConvertRect(Rect rect)
   // {
    //    return new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height);
   // }
}