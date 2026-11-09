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
        address.Should().Be(
            new Address(roadType, new RoadNumber(roadNumber), new RoadNumber(crossNumber), plateNumber));
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
    [InlineData("dgn 40 # 20-10", "DG 40 20 10")]
    [InlineData("TRANSVERSAL 5 #10 - 20", "TV 5 10 20")]
    [InlineData("transv 5 # 10-20", "TV 5 10 20")]
    [InlineData("tr. 5 # 10-20", "TV 5 10 20")]
    [InlineData("Avenida 68 # 13-45", "AV 68 13 45")]
    [InlineData("Avda. 68 # 13-45", "AV 68 13 45")]
    [InlineData("Ave 68 # 13-45", "AV 68 13 45")]
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

    [Fact]
    public void TryParse_BisOnRoadAndCross_ReturnsBisAndLetters()
    {
        var parsed = AddressParser.TryParse("calle 59 a bis b # 80 bis - 12", out var address);

        parsed.Should().BeTrue();
        address.Should().Be(new Address(
            RoadType.Calle,
            new RoadNumber(59, 'A', IsBis: true, BisLetter: 'B'),
            new RoadNumber(80, IsBis: true),
            12));
    }

    [Theory]
    [InlineData("calle 59 bis # 80-12", "CL 59 BIS 80 12")]
    [InlineData("calle 59bis # 80-12", "CL 59 BIS 80 12")]
    [InlineData("Calle 59 A Bis # 80-12", "CL 59A BIS 80 12")]
    [InlineData("calle 59 bis c # 80-12", "CL 59 BIS C 80 12")]
    [InlineData("CL 59A BIS B # 80-12", "CL 59A BIS B 80 12")]
    [InlineData("kr 7 # 32 bis - 16", "KR 7 32 BIS 16")]
    [InlineData("kr 7 # 32 a bis a - 16", "KR 7 32A BIS A 16")]
    public void TryParse_BisVariants_FormatsToStandardForm(string input, string expected)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        AddressFormatter.Format(address!).Should().Be(expected);
    }

    [Fact]
    public void TryParse_TrailingSurOnCarrera_AssignsSurToCross()
    {
        var parsed = AddressParser.TryParse("carrera 7 # 10-20 sur", out var address);

        parsed.Should().BeTrue();
        address.Should().Be(new Address(
            RoadType.Carrera,
            new RoadNumber(7),
            new RoadNumber(10, Quadrant: Quadrant.Sur),
            20));
    }

    [Theory]
    [InlineData("calle 26 sur # 50-10", "CL 26 SUR 50 10")]
    [InlineData("Calle 26 Sur # 50-10", "CL 26 SUR 50 10")]
    [InlineData("cl 26 s # 50-10", "CL 26 SUR 50 10")]
    [InlineData("calle 26 # 50-10 sur", "CL 26 SUR 50 10")]
    [InlineData("calle 59 a bis b sur # 80-12", "CL 59A BIS B SUR 80 12")]
    [InlineData("dg 40 s # 20-10", "DG 40 SUR 20 10")]
    [InlineData("ac 26 s # 50-10", "AC 26 SUR 50 10")]
    [InlineData("carrera 7 # 10-20 sur", "KR 7 10 20 SUR")]
    [InlineData("kr 7 # 10 sur - 20", "KR 7 10 20 SUR")]
    [InlineData("kr 7 sur # 10-20", "KR 7 10 20 SUR")]
    [InlineData("tv 5 # 10-20 sur", "TV 5 10 20 SUR")]
    [InlineData("carrera 5 este # 10-20", "KR 5 ESTE 10 20")]
    [InlineData("kr 5 # 10-20 este", "KR 5 ESTE 10 20")]
    [InlineData("ak 15 # 100-20 este", "AK 15 ESTE 100 20")]
    [InlineData("calle 10 # 5-20 este", "CL 10 5 20 ESTE")]
    [InlineData("calle 10 este # 5-20", "CL 10 5 20 ESTE")]
    [InlineData("calle 10 sur # 5-20 este", "CL 10 SUR 5 20 ESTE")]
    [InlineData("kr 5 e # 10-20 s", "KR 5 ESTE 10 20 SUR")]
    [InlineData("av 68 sur # 13-45", "AV 68 SUR 13 45")]
    [InlineData("av 68 # 13-45 sur", "AV 68 13 45 SUR")]
    [InlineData("calle 26 sur # 50-10 sur", "CL 26 SUR 50 10")]
    [InlineData("calle 26 sur sur # 50-10", "CL 26 SUR 50 10")]
    [InlineData("calle 26 # 50 sur - 10 sur", "CL 26 SUR 50 10")]
    [InlineData("kr 7 sur # 10-20 sur", "KR 7 10 20 SUR")]
    [InlineData("kr 5 este # 10 sur - 20 este sur", "KR 5 ESTE 10 20 SUR")]
    public void TryParse_QuadrantVariants_FormatsToStandardForm(string input, string expected)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        AddressFormatter.Format(address!).Should().Be(expected);
    }

    [Theory]
    [InlineData("Avenida Calle 26 # 68-35", "AC 26 68 35")]
    [InlineData("av. calle 26 # 68-35", "AC 26 68 35")]
    [InlineData("Av Cll 26 # 68-35", "AC 26 68 35")]
    [InlineData("avda cl 26 sur # 68-35", "AC 26 SUR 68 35")]
    [InlineData("Avenida Carrera 15 # 100-20", "AK 15 100 20")]
    [InlineData("av cra 15 # 100-20", "AK 15 100 20")]
    [InlineData("ave calle 26 # 68-35", "AC 26 68 35")]
    [InlineData("Av. Kr 68 # 13-45", "AK 68 13 45")]
    [InlineData("av carrera 30 # 1-30 sur", "AK 30 1 30 SUR")]
    public void TryParse_CompoundAvenueVariants_FormatsToStandardForm(string input, string expected)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        AddressFormatter.Format(address!).Should().Be(expected);
    }

    [Fact]
    public void TryParse_AddressWithoutComplements_ReturnsEmptyComplements()
    {
        var parsed = AddressParser.TryParse("calle 59 # 80-12", out var address);

        parsed.Should().BeTrue();
        address!.Complements.Should().BeEmpty();
    }

    [Fact]
    public void TryParse_TowerAndApartment_ReturnsComplementsInOrder()
    {
        var parsed = AddressParser.TryParse("calle 59 a bis sur # 80-12 torre 2 apto 302", out var address);

        parsed.Should().BeTrue();
        address.Should().Be(new Address(
            RoadType.Calle,
            new RoadNumber(59, 'A', IsBis: true, Quadrant: Quadrant.Sur),
            new RoadNumber(80),
            12)
        {
            Complements = [new(ComplementType.Torre, "2"), new(ComplementType.Apartamento, "302")],
        });
    }

    [Theory]
    [InlineData("calle 59 # 80-12 apartamento 302", ComplementType.Apartamento, "302")]
    [InlineData("calle 59 # 80-12 apto 302", ComplementType.Apartamento, "302")]
    [InlineData("calle 59 # 80-12 Apto. 302", ComplementType.Apartamento, "302")]
    [InlineData("calle 59 # 80-12 apt 302", ComplementType.Apartamento, "302")]
    [InlineData("calle 59 # 80-12 ap 302", ComplementType.Apartamento, "302")]
    [InlineData("calle 59 # 80-12 apto302", ComplementType.Apartamento, "302")]
    [InlineData("calle 59 # 80-12 apto 302 b", ComplementType.Apartamento, "302B")]
    [InlineData("calle 59 # 80-12 apto 302b", ComplementType.Apartamento, "302B")]
    [InlineData("calle 59 # 80-12 torre a", ComplementType.Torre, "A")]
    [InlineData("calle 59 # 80-12 to 3", ComplementType.Torre, "3")]
    [InlineData("calle 59 # 80-12 t 3", ComplementType.Torre, "3")]
    [InlineData("calle 59 # 80-12 t3", ComplementType.Torre, "3")]
    [InlineData("calle 59 # 80-12 interior 4", ComplementType.Interior, "4")]
    [InlineData("calle 59 # 80-12 int. 4", ComplementType.Interior, "4")]
    [InlineData("calle 59 # 80-12 in 4", ComplementType.Interior, "4")]
    [InlineData("calle 59 # 80-12 bloque c", ComplementType.Bloque, "C")]
    [InlineData("calle 59 # 80-12 blq 5", ComplementType.Bloque, "5")]
    [InlineData("calle 59 # 80-12 bl 5", ComplementType.Bloque, "5")]
    [InlineData("calle 59 # 80-12 bloque e", ComplementType.Bloque, "E")]
    [InlineData("calle 59 # 80-12 casa 15", ComplementType.Casa, "15")]
    [InlineData("calle 59 # 80-12 manzana 3", ComplementType.Manzana, "3")]
    [InlineData("calle 59 # 80-12 mz 3", ComplementType.Manzana, "3")]
    [InlineData("calle 59 # 80-12 mz. b", ComplementType.Manzana, "B")]
    [InlineData("calle 59 # 80-12 lote 7", ComplementType.Lote, "7")]
    [InlineData("calle 59 # 80-12 lt 7", ComplementType.Lote, "7")]
    [InlineData("carrera 7 # 10-20 sur apto 101", ComplementType.Apartamento, "101")]
    [InlineData("calle 59 # 80-12 apto 007", ComplementType.Apartamento, "007")]
    public void TryParse_SingleComplement_ReturnsTypeAndValue(string input, ComplementType type, string value)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        address!.Complements.Should().Equal(new Complement(type, value));
    }

    [Fact]
    public void TryParse_SeveralComplements_ReturnsAllInOrder()
    {
        var parsed = AddressParser.TryParse("kr 7 # 32-16 mz 3 lt 4 casa 2", out var address);

        parsed.Should().BeTrue();
        address!.Complements.Should().Equal(
            new Complement(ComplementType.Manzana, "3"),
            new Complement(ComplementType.Lote, "4"),
            new Complement(ComplementType.Casa, "2"));
    }

    [Theory]
    [InlineData("calle 59 # 80-12 conjunto los pinos", ComplementType.Conjunto, "LOS PINOS")]
    [InlineData("calle 59 # 80-12 conj. Los Pinos", ComplementType.Conjunto, "LOS PINOS")]
    [InlineData("calle 59 # 80-12 cj villa del río 2", ComplementType.Conjunto, "VILLA DEL RIO 2")]
    [InlineData("calle 59 # 80-12 conjunto residencial los pinos", ComplementType.ConjuntoResidencial, "LOS PINOS")]
    [InlineData("calle 59 # 80-12 conj. res. los pinos", ComplementType.ConjuntoResidencial, "LOS PINOS")]
    public void TryParse_ConjuntoName_ReturnsNameAsValue(string input, ComplementType type, string value)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeTrue();
        address!.Complements.Should().Equal(new Complement(type, value));
    }

    [Fact]
    public void TryParse_ConjuntoFollowedByComplements_EndsNameAtNextKeyword()
    {
        var parsed = AddressParser.TryParse("kr 7 # 32-16 conjunto los pinos torre 2 apto 302", out var address);

        parsed.Should().BeTrue();
        address!.Complements.Should().Equal(
            new Complement(ComplementType.Conjunto, "LOS PINOS"),
            new Complement(ComplementType.Torre, "2"),
            new Complement(ComplementType.Apartamento, "302"));
    }

    [Fact]
    public void TryParse_KeywordLetterAfterValue_StartsNewComplement()
    {
        var parsed = AddressParser.TryParse("calle 59 # 80-12 apto 302 t 2", out var address);

        parsed.Should().BeTrue();
        address!.Complements.Should().Equal(
            new Complement(ComplementType.Apartamento, "302"),
            new Complement(ComplementType.Torre, "2"));
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
    [InlineData("calle bis # 80-12")]
    [InlineData("calle 59 bis bis # 80-12")]
    [InlineData("calle 59 bis a b # 80-12")]
    [InlineData("calle 59 # 80-12 bis")]
    [InlineData("calle sur # 50-10")]
    [InlineData("av 68 sur este # 13-45")]
    [InlineData("calle carrera 26 # 68-35")]
    [InlineData("avenida avenida 26 # 68-35")]
    [InlineData("av diagonal 40 # 20-10")]
    [InlineData("av calle carrera 26 # 68-35")]
    [InlineData("calle 59 # 80-12 apto")]
    [InlineData("calle 59 # 80-12 apto torre 2")]
    [InlineData("calle 59 # 80-12 apto 302 b c")]
    [InlineData("calle 59 # 80-12 apto 302 302")]
    [InlineData("calle 59 # 80-12 apto ab")]
    [InlineData("calle 59 # 80-12 apto 302 sur")]
    [InlineData("calle 59 # 80-12 apto 302 # 5")]
    [InlineData("calle 59 # 80-12 ca 15")]
    [InlineData("calle 59 # 80-12 cs 15")]
    [InlineData("calle 59 # 80-12 conjunto")]
    [InlineData("calle 59 # 80-12 conjunto residencial")]
    [InlineData("calle 59 # 80-12 conjunto apto 302")]
    [InlineData("calle 59 # 80-12 conjunto los-pinos")]
    [InlineData("mz 3 lt 4")]
    public void TryParse_InvalidAddress_ReturnsFalse(string input)
    {
        var parsed = AddressParser.TryParse(input, out var address);

        parsed.Should().BeFalse();
        address.Should().BeNull();
    }
}
