using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class RoadTypeKeywordsTests
{
    [Theory]
    [InlineData("calle", RoadType.Calle)]
    [InlineData("cll", RoadType.Calle)]
    [InlineData("cl", RoadType.Calle)]
    [InlineData("carrera", RoadType.Carrera)]
    [InlineData("cra", RoadType.Carrera)]
    [InlineData("kr", RoadType.Carrera)]
    [InlineData("diagonal", RoadType.Diagonal)]
    [InlineData("dg", RoadType.Diagonal)]
    [InlineData("transversal", RoadType.Transversal)]
    [InlineData("tv", RoadType.Transversal)]
    [InlineData("avenida", RoadType.Avenida)]
    [InlineData("av", RoadType.Avenida)]
    [InlineData("ac", RoadType.AvenidaCalle)]
    [InlineData("ak", RoadType.AvenidaCarrera)]
    public void TryGetRoadType_KnownKeyword_ReturnsRoadType(string keyword, RoadType expected)
    {
        var found = RoadTypeKeywords.TryGetRoadType(keyword, out var roadType);

        found.Should().BeTrue();
        roadType.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("apto")]
    [InlineData("59")]
    public void TryGetRoadType_UnknownKeyword_ReturnsFalse(string keyword)
    {
        var found = RoadTypeKeywords.TryGetRoadType(keyword, out _);

        found.Should().BeFalse();
    }
}
