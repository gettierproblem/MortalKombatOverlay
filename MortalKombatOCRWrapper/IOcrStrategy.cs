using System.Drawing;
using Emgu.CV;

namespace MortalKombatOCRWrapper;

public interface IOcrStrategy
{
    string ExtractTextFromImage(UMat image, Rectangle roi);

    void Preprocess(UMat thresholded);
}