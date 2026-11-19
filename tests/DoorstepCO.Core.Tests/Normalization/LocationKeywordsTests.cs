using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class LocationKeywordsTests
{
    [Theory]
    [InlineData("bogota", 0)]
    [InlineData("bogota d c", 0)]
    [InlineData("bogota dc", 0)]
    [InlineData("medellin antioquia", 0)]
    [InlineData("santa marta magdalena colombia", 0)]
    [InlineData("cali valle del cauca", 0)]
    [InlineData("apto 302 bucaramanga", 2)]
    [InlineData("frente al parque cartagena", 3)]
    public void IsLocationSuffix_OnlyLocationsFromStart_ReturnsTrue(string text, int start)
    {
        LocationKeywords.IsLocationSuffix(text.Split(' '), start).Should().BeTrue();
    }

    [Theory]
    [InlineData("frente al parque", 0)]
    [InlineData("bogota frente al parque", 0)]
    [InlineData("valle del lili", 0)]
    [InlineData("santa", 0)]
    [InlineData("medellin", 1)]
    public void IsLocationSuffix_NotOnlyLocations_ReturnsFalse(string text, int start)
    {
        LocationKeywords.IsLocationSuffix(text.Split(' '), start).Should().BeFalse();
    }
}
