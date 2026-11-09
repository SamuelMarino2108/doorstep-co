using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class QuadrantKeywordsTests
{
    [Theory]
    [InlineData("sur", Quadrant.Sur)]
    [InlineData("s", Quadrant.Sur)]
    [InlineData("este", Quadrant.Este)]
    [InlineData("e", Quadrant.Este)]
    public void TryGetQuadrant_KnownKeyword_ReturnsQuadrant(string keyword, Quadrant expected)
    {
        var found = QuadrantKeywords.TryGetQuadrant(keyword, out var quadrant);

        found.Should().BeTrue();
        quadrant.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("norte")]
    [InlineData("oeste")]
    public void TryGetQuadrant_UnknownKeyword_ReturnsFalse(string keyword)
    {
        var found = QuadrantKeywords.TryGetQuadrant(keyword, out _);

        found.Should().BeFalse();
    }
}
