using FluentAssertions;
using MortalKombatOCRWrapper;

namespace MortalKombatOverlayTest;

[TestClass]
public class OcrCharacterNameTest
{
    [TestMethod]
    public void ExtractNamesFrom1080pLiMei_ShouldReturnLiMeiAndLiuKang()
    {
        var imagePath = "POC\\Li Mei 1080p.png";
        var expectedNames = new List<string> { "LI MEI", "LIU KANG" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom1920x1440LiMei_ShouldReturnReikoAndLiMei()
    {
        var imagePath = "POC\\Reiko 1920x1440.png";
        var expectedNames = new List<string> { "REIKO", "LI MEI" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kJohnnyCageImage_ShouldReturnJohnnyCageAndGeneralShao()
    {
        var imagePath = "POC\\Johnny Cage.png";
        var expectedNames = new List<string> { "JOHNNY CAGE", "GENERAL SHAO" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kKenshiImage_ShouldReturnKenshiAndBaraka()
    {
        var imagePath = "POC\\Kenshi.png";
        var expectedNames = new List<string> { "KENSHI", "BARAKA" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kLiMeiImage_ShouldReturnLiMeiAndSmoke()
    {
        var imagePath = "POC\\Li Mei.png";
        var expectedNames = new List<string> { "LI MEI", "SMOKE" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kRaidenImage_ShouldReturnRaidenAndNitara()
    {
        var imagePath = "POC\\Raiden.png";
        var expectedNames = new List<string> { "RAIDEN", "NITARA" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kReikoImage_ShouldReturnReikoAndRain()
    {
        var imagePath = "POC\\Reiko.png";
        var expectedNames = new List<string> { "REIKO", "RAIN" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kSelectImage_ShouldReturnEmptyList()
    {
        // this is a negative test, if you get results here the ocr is hallucinating
        var imagePath = "POC\\Select.png";
        var expectedNames = new List<string> { "", "" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kShangTsungImage_ShouldReturnShangTsungAndScorpion()
    {
        var imagePath = "POC\\Shang Tsung.png";
        var expectedNames = new List<string> { "SHANG TSUNG", "SCORPION" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kSindelImage_ShouldReturnSindelAndTanya()
    {
        var imagePath = "POC\\Sindel.png";
        var expectedNames = new List<string> { "SINDEL", "TANYA" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFrom4kSubZeroImage_ShouldReturnSubZeroAndMileena()
    {
        var imagePath = "POC\\Sub-Zero.png";
        var expectedNames = new List<string> { "SUB-ZERO", "MILEENA" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }

    [TestMethod]
    public void ExtractNamesFromWeirdAspectTanyaShouldReturnReikoAndLiMei()
    {
        var imagePath = "POC\\TanyaWeirdAspect.png";
        var expectedNames = new List<string> { "TANYA", "LI MEI" };
        var names = OcrHelper.ExtractCharacterNames(imagePath, new PaddleOcrStrategy());

        names.Should().BeEquivalentTo(expectedNames);
    }
}