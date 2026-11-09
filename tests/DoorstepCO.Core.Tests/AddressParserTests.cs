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
        address.Should().Be(new Address(roadType, new RoadNumber(roadNumber), new RoadNumber(crossNumber), plateNumber));
    }

    [Fact]
    public void TryParse_LettersOnRoadAndCross_ReturnsLetters()
    {
        var parsed = AddressParser.TryParse("calle 59a # 80 b - 12", out var address);

        parsed.Should().BeTrue();
        address.Should().Be(new Address(RoadType.Calle, new RoadNumber(59, 'A'), new RoadNumber(80, 'B'), 12));
    }

    [Theory]
    [InlineData("calle 59 # 80-12", "CL 59 80 12")]
    [InlineData("Calle 59 #80-12", "CL 59 80 12")]
    [InlineData("CLL 59 # 80 - 12", "CL 59 80 12")]
    [InlineData("cl. 59 no. 80-12", "CL 59 80 12")]
    [InlineData("Clle 59 N° 80 12", "CL 59 80 12")]
    [InlineData("Cálle 59 Nº 80-12", "CL 59 80 12")]
    [InlineData("  calle   59   #   80  -  12  ", "CL 59 80 12")]
    [InlineData("calle59#80-12", "CL 59 80 12")]
    [InlineData("Cra. 7 No. 32-16", "KR 7 32 16")]
    [InlineData("Carrera 7 numero 32-16", "KR 7 32 16")]
    [InlineData("KRA 7 Nro. 32-16", "KR 7 32 16")]
    [InlineData("kr 7 n 32 16", "KR 7 32 16")]
    [InlineData("cr 7 num 32-16", "KR 7 32 16")]
    [InlineData("carr. 7 # 32-16", "KR 7 32 16")]
    [InlineData("Diagonal 40 nro 20-10", "DG 40 20 10")]
    [InlineData("diag. 40 # 20-10", "DG 40 20 10")]
    [InlineData("DG 40 # 20-10", "DG 40 20 10")]
    [InlineData("TRANSVERSAL 5 #10 - 20", "TV 5 10 20")]
    [InlineData("transv 5 # 10-20", "TV 5 10 20")]
    [InlineData("tr. 5 # 10-20", "TV 5 10 20")]
    [InlineData("Avenida 68 # 13-45", "AV 68 13 45")]
    [InlineData("Avda. 68 # 13-45", "AV 68 13 45")]
    [InlineData("AC 26 # 50-01", "AC 26 50 1")]
    [InlineData("ak 15 # 100-20", "AK 15 100 20")]
    public void TryParse_KeywordAndSeparatorVariants_FormatsToStandardForm(string input, string expected)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        AddressFormatter.Format(address!).Should().Be(expected);
    }

    [Theory]
    [InlineData("calle 59a # 80-12", "CL 59A 80 12")]
    [InlineData("Calle 59 A # 80-12", "CL 59A 80 12")]
    [InlineData("calle 59A #80B-12", "CL 59A 80B 12")]
    [InlineData("kr 7 # 32 c - 16", "KR 7 32C 16")]
    [InlineData("cl 26 b 32 10", "CL 26B 32 10")]
    [InlineData("kr 7 n 32 16", "KR 7 32 16")]
    public void TryParse_LetterVariants_FormatsToStandardForm(string input, string expected)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        AddressFormatter.Format(address!).Should().Be(expected);
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
    [InlineData("calle 59 ab # 80-12")]
    [InlineData("calle 59 a b # 80-12")]
    [InlineData("calle 59 # 80-12 a")]
    [InlineData("calle a # 80-12")]
    public void TryParse_InvalidAddress_ReturnsFalse(string input)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeFalse();
        address.Should().BeNull();
    }
}
