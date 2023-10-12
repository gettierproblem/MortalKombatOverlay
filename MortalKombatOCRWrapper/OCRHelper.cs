using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace MortalKombatOCRWrapper;

public class OcrHelper
{
    public static List<string> ExtractCharacterNames(Bitmap bitmap)
    {
        var image = bitmap.ToImage<Bgr, byte>().ToUMat(); // Convert Bitmap to UMat
        return ExtractCharacterNames(image, new PaddleOcrStrategy());
    }

    public static List<string> ExtractCharacterNames(string imagePath, IOcrStrategy? strategy = null)
    {
        strategy ??= new PaddleOcrStrategy();

        var image = new UMat(imagePath, ImreadModes.Color);
        return ExtractCharacterNames(image, strategy);
    }

    public static List<string> ExtractCharacterNames(UMat image, IOcrStrategy? ocrStrategy)
    {
        var gameRect = FindGameScreenBounds(image);

        var processed = new UMat(image, gameRect);

        ocrStrategy.Preprocess(processed);

        var playerNames = new List<string>();

        var imageWidth = processed.Cols;
        var imageHeight = processed.Rows;

        // only use the ocr on the regions of the screen where the player names are
        var player1 = new Rectangle((int)(0.08 * imageWidth), (int)(0.05 * imageHeight), (int)(0.2 * imageWidth),
            (int)(0.1 * imageHeight));
        var player2 = new Rectangle((int)(0.75 * imageWidth), (int)(0.05 * imageHeight), (int)(0.2 * imageWidth),
            (int)(0.1 * imageHeight));

        foreach (var roi in new[] { player1, player2 })
        {
            var recognizedText = ocrStrategy.ExtractTextFromImage(processed, roi);
            playerNames.Add(recognizedText);
        }

        return playerNames;
    }

    public static string FindClosestMatch(string input, List<string> possibleNames)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 3) return string.Empty;

        // Prioritize exact matches
        if (possibleNames.Contains(input)) return input;

        // we want to make sure at least a couple of characters match ocr output
        // because the select screen may read random garbage
        var bestDistance = input.Length - 2;

        var bestMatch = string.Empty;

        foreach (var name in possibleNames)
        {
            var distance = ComputeLevenshteinDistance(input, name);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMatch = name;
            }
            else if (distance == bestDistance)
            {
                // Tie-breaker: Choose the shorter name
                if (name.Length < bestMatch.Length) bestMatch = name;
            }
        }

        return bestMatch;
    }

    // mortal kombat is always 16:9 and puts black bars around the game if the window aspect ratio is different
    // so cut the bars off before getting the rois containing the player names
    public static Rectangle FindGameScreenBounds(UMat image)
    {
        var currentAspectRatio = (double)image.Cols / image.Rows;

        // expected dimensions based on 16:9 ratio
        var expectedAspectRatio = 16.0 / 9.0;


        // for 16:9 or greater return the whole image bounds
        if (currentAspectRatio > expectedAspectRatio || Math.Abs(currentAspectRatio - 16.0 / 9.0) < 0.1)
            return new Rectangle(0, 0, image.Cols, image.Rows);

        var grayImage = new UMat();
        CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

        var threshold = 10;

        // the window chrome is included in the screenshot so just go 50 pixels down
        // and hope we don't hit the top of the game screen
        var top = 50;

        var initRowSum = CvInvoke.Sum(grayImage.Row(top));
        if (initRowSum.V0 < threshold * grayImage.Cols)
            for (; top < grayImage.Rows; top++)
            {
                var rowSum = CvInvoke.Sum(grayImage.Row(top));
                if (rowSum.V0 > threshold * grayImage.Cols)
                    break;
            }
        else
            top = 0; // no bar found

        var left = 3; // approximate size of the window chrome

        var initColSum = CvInvoke.Sum(grayImage.Col(left));

        if (initColSum.V0 < threshold * grayImage.Rows)
            for (; left < grayImage.Cols; left++)
            {
                var colSum = CvInvoke.Sum(grayImage.Col(left));
                if (colSum.V0 > threshold * grayImage.Rows)
                    break;
            }
        else
            left = 0; // no bar found


        var expectedHeight = (int)(grayImage.Cols / expectedAspectRatio);

        var bottom = top + expectedHeight;
        var right = left + (int)(expectedHeight * expectedAspectRatio);

        return new Rectangle(left, top, right - left, bottom - top);
    }

    private static int ComputeLevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return !string.IsNullOrEmpty(b) ? b.Length : 0;

        if (string.IsNullOrEmpty(b)) return a.Length;

        var lengthA = a.Length;
        var lengthB = b.Length;
        var distances = new int[lengthA + 1, lengthB + 1];

        // Initialize the first row and column
        for (var i = 0; i <= lengthA; distances[i, 0] = i++)
        {
        }

        for (var j = 0; j <= lengthB; distances[0, j] = j++)
        {
        }

        for (var i = 1; i <= lengthA; i++)
        for (var j = 1; j <= lengthB; j++)
        {
            var cost = b[j - 1] == a[i - 1] ? 0 : 1;
            distances[i, j] = Math.Min(
                Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                distances[i - 1, j - 1] + cost);
        }

        return distances[lengthA, lengthB];
    }


}