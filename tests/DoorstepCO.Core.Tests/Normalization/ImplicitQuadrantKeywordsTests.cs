using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class ImplicitQuadrantKeywordsTests
{
    [Fact]
    public void IsImplicitQuadrant_Norte_ReturnsTrue()
    {
        ImplicitQuadrantKeywords.IsImplicitQuadrant("norte").Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("n")]
    [InlineData("sur")]
    [InlineData("oeste")]
    public void IsImplicitQuadrant_OtherToken_ReturnsFalse(string token)
    {
        ImplicitQuadrantKeywords.IsImplicitQuadrant(token).Should().BeFalse();
    }
}
