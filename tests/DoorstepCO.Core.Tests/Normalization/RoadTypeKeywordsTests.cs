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
    [InlineData("dgn", RoadType.Diagonal)]
    [InlineData("transversal", RoadType.Transversal)]
    [InlineData("tv", RoadType.Transversal)]
    [InlineData("avenida", RoadType.Avenida)]
    [InlineData("av", RoadType.Avenida)]
    [InlineData("ave", RoadType.Avenida)]
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

    [Theory]
    [InlineData(RoadType.Avenida, RoadType.Calle, RoadType.AvenidaCalle)]
    [InlineData(RoadType.Avenida, RoadType.Carrera, RoadType.AvenidaCarrera)]
    public void TryCombine_AvenidaWithCalleOrCarrera_ReturnsCompoundRoadType(
        RoadType first, RoadType second, RoadType expected)
    {
        var combined = RoadTypeKeywords.TryCombine(first, second, out var roadType);

        combined.Should().BeTrue();
        roadType.Should().Be(expected);
    }

    [Theory]
    [InlineData(RoadType.Calle, RoadType.Carrera)]
    [InlineData(RoadType.Avenida, RoadType.Avenida)]
    [InlineData(RoadType.Avenida, RoadType.Diagonal)]
    [InlineData(RoadType.Avenida, RoadType.AvenidaCalle)]
    public void TryCombine_OtherPair_ReturnsFalse(RoadType first, RoadType second)
    {
        var combined = RoadTypeKeywords.TryCombine(first, second, out _);

        combined.Should().BeFalse();
    }
}
