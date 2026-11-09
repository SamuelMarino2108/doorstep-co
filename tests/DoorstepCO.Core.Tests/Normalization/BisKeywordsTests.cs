using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class BisKeywordsTests
{
    [Fact]
    public void IsBis_Bis_ReturnsTrue()
    {
        BisKeywords.IsBis("bis").Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("bi")]
    [InlineData("sur")]
    public void IsBis_OtherToken_ReturnsFalse(string token)
    {
        BisKeywords.IsBis(token).Should().BeFalse();
    }
}
