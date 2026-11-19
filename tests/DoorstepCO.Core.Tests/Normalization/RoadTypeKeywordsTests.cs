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
    [InlineData("k", RoadType.Carrera)]
    [InlineData("autopista", RoadType.Autopista)]
    [InlineData("autop", RoadType.Autopista)]
    [InlineData("au", RoadType.Autopista)]
    [InlineData("circular", RoadType.Circular)]
    [InlineData("circ", RoadType.Circular)]
    [InlineData("cq", RoadType.Circular)]
    [InlineData("circunvalar", RoadType.Circunvalar)]
    [InlineData("circunv", RoadType.Circunvalar)]
    [InlineData("cv", RoadType.Circunvalar)]
    public void TryGetRoadType_KnownKeyword_ReturnsRoadType(string keyword, RoadType expected)
    {
        var found = RoadTypeKeywords.TryGetRoadType(keyword, out var roadType);

        found.Should().BeTrue();
        roadType.Should().Be(expected);
    }

    [Theory]
    [InlineData("carrerra", RoadType.Carrera)]
    [InlineData("carerra", RoadType.Carrera)]
    [InlineData("calel", RoadType.Calle)]
    [InlineData("callle", RoadType.Calle)]
    [InlineData("diagonl", RoadType.Diagonal)]
    [InlineData("transversl", RoadType.Transversal)]
    [InlineData("avenda", RoadType.Avenida)]
    [InlineData("autopsta", RoadType.Autopista)]
    public void TryCorrectRoadType_MisspelledKeyword_ReturnsRoadType(string word, RoadType expected)
    {
        var found = RoadTypeKeywords.TryCorrectRoadType(word, out var roadType);

        found.Should().BeTrue();
        roadType.Should().Be(expected);
    }

    [Theory]
    [InlineData("valle")]
    [InlineData("barrio")]
    [InlineData("cale")]
    [InlineData("carrera")]
    [InlineData("conjunto")]
    public void TryCorrectRoadType_ExactOrUnrelatedWord_ReturnsFalse(string word)
    {
        RoadTypeKeywords.TryCorrectRoadType(word, out _).Should().BeFalse();
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
