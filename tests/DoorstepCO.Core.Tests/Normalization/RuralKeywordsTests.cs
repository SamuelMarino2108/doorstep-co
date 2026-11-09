using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class RuralKeywordsTests
{
    [Theory]
    [InlineData("vereda")]
    [InlineData("finca")]
    [InlineData("km")]
    [InlineData("kilometro")]
    [InlineData("corregimiento")]
    [InlineData("hacienda")]
    [InlineData("parcela")]
    public void IsRural_RuralKeyword_ReturnsTrue(string token)
    {
        RuralKeywords.IsRural(token).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("calle")]
    [InlineData("via")]
    [InlineData("k")]
    [InlineData("apto")]
    public void IsRural_OtherToken_ReturnsFalse(string token)
    {
        RuralKeywords.IsRural(token).Should().BeFalse();
    }
}
