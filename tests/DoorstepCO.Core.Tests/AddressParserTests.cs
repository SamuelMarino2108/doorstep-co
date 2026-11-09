namespace DoorstepCO.Core.Tests;

public sealed class AddressParserTests
{
    [Theory]
    [InlineData("calle 59 # 80-12", RoadType.Calle, 59, 80, 12)]
    [InlineData("Cra. 7 No. 32-16", RoadType.Carrera, 7, 32, 16)]
    [InlineData("kr 7 n 32 16", RoadType.Carrera, 7, 32, 16)]
    [InlineData("Diagonal 40 nro 20-10", RoadType.Diagonal, 40, 20, 10)]
    [InlineData("TRANSVERSAL 5 #10 - 20", RoadType.Transversal, 5, 10, 20)]
    [InlineData("av 68 # 13-45", RoadType.Avenida, 68, 13, 45)]
    [InlineData("ak 15 # 100-20", RoadType.AvenidaCarrera, 15, 100, 20)]
    [InlineData("cl 26 32 10", RoadType.Calle, 26, 32, 10)]
    public void TryParse_ValidAddress_ReturnsComponents(
        string input, RoadType roadType, int roadNumber, int crossNumber, int plateNumber)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        address.Should().Be(new Address(roadType, roadNumber, crossNumber, plateNumber));
    }

    [Theory]
    [InlineData("")]
    [InlineData("59 # 80-12")]
    [InlineData("calle # 80-12")]
    [InlineData("calle 59 # 80")]
    [InlineData("calle 59 # 80-")]
    [InlineData("calle 59 # # 80-12")]
    [InlineData("calle 59 # 80-12-5")]
    [InlineData("calle abc # 80-12")]
    [InlineData("calle 99999999999 # 80-12")]
    public void TryParse_InvalidAddress_ReturnsFalse(string input)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeFalse();
        address.Should().BeNull();
    }
}
