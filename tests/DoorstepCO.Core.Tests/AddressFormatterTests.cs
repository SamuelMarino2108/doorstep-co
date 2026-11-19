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
    [InlineData(RoadType.Autopista, 45, 128, 50, "AU 45 128 50")]
    [InlineData(RoadType.Circular, 1, 70, 50, "CQ 1 70 50")]
    [InlineData(RoadType.Circunvalar, 5, 20, 30, "CV 5 20 30")]
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
        var address = new Address(RoadType.Calle, new RoadNumber(59, "A"), new RoadNumber(80, "B"), 12);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("CL 59A 80B 12");
    }

    [Fact]
    public void Format_AddressWithDoubleLetter_AppendsBothLetters()
    {
        var address = new Address(RoadType.Diagonal, new RoadNumber(74, "B"), new RoadNumber(33, "AA"), 19);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("DG 74B 33AA 19");
    }

    [Fact]
    public void Format_AddressWithBis_WritesBisAndBisLetter()
    {
        var address = new Address(
            RoadType.Calle,
            new RoadNumber(59, "A", IsBis: true, BisLetter: 'B'),
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
    [InlineData(ComplementType.Oficina, "301", "CL 59 80 12 OF 301")]
    [InlineData(ComplementType.Local, "5", "CL 59 80 12 LC 5")]
    [InlineData(ComplementType.Piso, "3", "CL 59 80 12 PI 3")]
    [InlineData(ComplementType.Bodega, "4", "CL 59 80 12 BG 4")]
    [InlineData(ComplementType.Consultorio, "201", "CL 59 80 12 CS 201")]
    [InlineData(ComplementType.Etapa, "2", "CL 59 80 12 ET 2")]
    [InlineData(ComplementType.Sector, "3", "CL 59 80 12 SC 3")]
    [InlineData(ComplementType.Edificio, "EL ROBLE", "CL 59 80 12 ED EL ROBLE")]
    [InlineData(ComplementType.Barrio, "CHAPINERO", "CL 59 80 12 BR CHAPINERO")]
    [InlineData(ComplementType.Urbanizacion, "LOS PINOS", "CL 59 80 12 UR LOS PINOS")]
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

    [Fact]
    public void Format_AddressWithNamedRoad_WritesNameInsteadOfNumber()
    {
        var address = new Address(
            RoadType.Avenida, new RoadNumber(0, Quadrant: Quadrant.Sur, Name: "EL POBLADO"), new RoadNumber(30), 28);

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("AV EL POBLADO SUR 30 28");
    }

    [Fact]
    public void Format_AddressWithRemarks_WritesRemarksLast()
    {
        var address = new Address(RoadType.Calle, new RoadNumber(10), new RoadNumber(5), 51)
        {
            Complements = [new(ComplementType.Apartamento, "302")],
            Remarks = "FRENTE AL PARQUE",
        };

        var formatted = AddressFormatter.Format(address);

        formatted.Should().Be("CL 10 5 51 AP 302 FRENTE AL PARQUE");
    }
}
