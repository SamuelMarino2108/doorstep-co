using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class TypoCorrectionTests
{
    [Theory]
    [InlineData("carrerra", "carrera")]
    [InlineData("carera", "carrera")]
    [InlineData("cerrera", "carrera")]
    [InlineData("calel", "calle")]
    [InlineData("diagnoal", "diagonal")]
    [InlineData("calle", "calle")]
    public void IsWithinOneEdit_OneEditOrLess_ReturnsTrue(string word, string keyword)
    {
        TypoCorrection.IsWithinOneEdit(word, keyword).Should().BeTrue();
    }

    [Theory]
    [InlineData("carrera", "calle")]
    [InlineData("carrrrera", "carrera")]
    [InlineData("cra", "carrera")]
    [InlineData("acrrrea", "carrera")]
    public void IsWithinOneEdit_MoreThanOneEdit_ReturnsFalse(string word, string keyword)
    {
        TypoCorrection.IsWithinOneEdit(word, keyword).Should().BeFalse();
    }
}
