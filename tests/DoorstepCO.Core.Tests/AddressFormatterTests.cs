namespace DoorstepCO.Core.Tests;

public sealed class AddressFormatterTests
{
    [Theory]
    [InlineData(RoadType.Calle, 59, 80, 12, "CL 59 80 12")]
    [InlineData(RoadType.Carrera, 7, 32, 16, "CRA 7 32 16")]
    [InlineData(RoadType.Diagonal, 40, 20, 10, "DG 40 20 10")]
    [InlineData(RoadType.Transversal, 5, 10, 20, "TV 5 10 20")]
    [InlineData(RoadType.Avenida, 68, 13, 45, "AV 68 13 45")]
    [InlineData(RoadType.AvenidaCalle, 26, 50, 1, "AC 26 50 1")]
    [InlineData(RoadType.AvenidaCarrera, 15, 100, 20, "AK 15 100 20")]
    public void Format_Address_ReturnsStandardForm(
        RoadType roadType, int roadNumber, int crossNumber, int plateNumber, string expected)
    {
        var address = new Address(roadType, new RoadNumber(roadNumber), new RoadNumber(crossNumber), plateNumber);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be(expected);
    }

    [Fact]
    public void Format_AddressWithLetters_AppendsLettersToNumbers()
    {
        var address = new Address(RoadType.Calle, new RoadNumber(59, 'A'), new RoadNumber(80, 'B'), 12);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("CL 59A 80B 12");
    }

    [Fact]
    public void Format_AddressWithBis_WritesBisAndBisLetter()
    {
        var address = new Address(
            RoadType.Calle,
            new RoadNumber(59, 'A', IsBis: true, BisLetter: 'B'),
            new RoadNumber(80, IsBis: true),
            12);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("CL 59A BIS B 80 BIS 12");
    }

    [Fact]
    public void Format_AddressWithQuadrants_WritesRoadQuadrantInlineAndCrossQuadrantLast()
    {
        var address = new Address(
            RoadType.Calle,
            new RoadNumber(10, Quadrant: Quadrant.Sur),
            new RoadNumber(5, Quadrant: Quadrant.Este),
            20);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("CL 10 SUR 5 20 ESTE");
    }

    [Theory]
    [InlineData(ComplementType.Apartamento, "302", "CL 59 80 12 AP 302")]
    [InlineData(ComplementType.Torre, "B", "CL 59 80 12 TO B")]
    [InlineData(ComplementType.Interior, "4", "CL 59 80 12 IN 4")]
    [InlineData(ComplementType.Bloque, "5", "CL 59 80 12 BL 5")]
    [InlineData(ComplementType.Casa, "15", "CL 59 80 12 CA 15")]
    [InlineData(ComplementType.Manzana, "3", "CL 59 80 12 MZ 3")]
    [InlineData(ComplementType.Lote, "7", "CL 59 80 12 LT 7")]
    [InlineData(ComplementType.Conjunto, "LOS PINOS", "CL 59 80 12 CONJ LOS PINOS")]
    [InlineData(ComplementType.ConjuntoResidencial, "LOS PINOS", "CL 59 80 12 CON LOS PINOS")]
    public void Format_AddressWithComplement_AppendsDianAbbreviationAndValue(
        ComplementType type, string value, string expected)
    {
        var address = new Address(RoadType.Calle, new RoadNumber(59), new RoadNumber(80), 12)
        {
            Complements = [new(type, value)],
        };

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be(expected);
    }

    [Fact]
    public void Format_AddressWithQuadrantAndComplements_WritesComplementsAfterCrossQuadrant()
    {
        var address = new Address(
            RoadType.Carrera,
            new RoadNumber(7),
            new RoadNumber(10, Quadrant: Quadrant.Sur),
            20)
        {
            Complements = [new(ComplementType.Torre, "2"), new(ComplementType.Apartamento, "302")],
        };

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("CRA 7 10 20 SUR TO 2 AP 302");
    }
}
