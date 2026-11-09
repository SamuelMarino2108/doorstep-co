using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class NamedAvenuesTests
{
    [Theory]
    [InlineData("el dorado", RoadType.AvenidaCalle, 26)]
    [InlineData("eldorado", RoadType.AvenidaCalle, 26)]
    [InlineData("jorge eliecer gaitan", RoadType.AvenidaCalle, 26)]
    [InlineData("caracas", RoadType.AvenidaCarrera, 14)]
    [InlineData("boyaca", RoadType.AvenidaCarrera, 72)]
    [InlineData("nqs", RoadType.AvenidaCarrera, 30)]
    [InlineData("ciudad de quito", RoadType.AvenidaCarrera, 30)]
    [InlineData("chile", RoadType.AvenidaCalle, 72)]
    [InlineData("congreso eucaristico", RoadType.AvenidaCarrera, 68)]
    [InlineData("ciudad de lima", RoadType.AvenidaCalle, 19)]
    [InlineData("comuneros", RoadType.AvenidaCalle, 6)]
    [InlineData("de los comuneros", RoadType.AvenidaCalle, 6)]
    [InlineData("pepe sierra", RoadType.AvenidaCalle, 116)]
    [InlineData("la esperanza", RoadType.AvenidaCalle, 24)]
    public void TryGetRoad_KnownName_ReturnsStandardRoad(string name, RoadType expectedType, int expectedNumber)
    {
        var found = NamedAvenues.TryGetRoad(name, out var roadType, out var road);

        found.Should().BeTrue();
        roadType.Should().Be(expectedType);
        road.Should().Be(new RoadNumber(expectedNumber));
    }

    [Fact]
    public void TryGetRoad_PrimeroDeMayo_ReturnsSouthQuadrant()
    {
        var found = NamedAvenues.TryGetRoad("primero de mayo", out var roadType, out var road);

        found.Should().BeTrue();
        roadType.Should().Be(RoadType.AvenidaCalle);
        road.Should().Be(new RoadNumber(22, Quadrant: Quadrant.Sur));
    }

    [Theory]
    [InlineData("")]
    [InlineData("el poblado")]
    [InlineData("americas")]
    public void TryGetRoad_UnknownName_ReturnsFalse(string name)
    {
        var found = NamedAvenues.TryGetRoad(name, out _, out _);

        found.Should().BeFalse();
    }
}
