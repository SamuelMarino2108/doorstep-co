using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class PlateSeparatorKeywordsTests
{
    [Theory]
    [InlineData("#")]
    [InlineData("no")]
    [InlineData("nro")]
    [InlineData("n")]
    [InlineData("num")]
    [InlineData("numero")]
    public void IsPlateSeparator_KnownSeparator_ReturnsTrue(string token)
    {
        PlateSeparatorKeywords.IsPlateSeparator(token).Should().BeTrue();
    }

    [Theory]
    [InlineData("-")]
    [InlineData("calle")]
    [InlineData("80")]
    public void IsPlateSeparator_OtherToken_ReturnsFalse(string token)
    {
        PlateSeparatorKeywords.IsPlateSeparator(token).Should().BeFalse();
    }
}
