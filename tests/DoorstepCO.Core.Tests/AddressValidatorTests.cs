namespace DoorstepCO.Core.Tests;

public sealed class AddressValidatorTests
{
    [Theory]
    [InlineData("cll 59 a bis sur # 80-12 apto 302", "CL 59A BIS SUR 80 12 AP 302")]
    [InlineData("Cra. 7 No. 32-16", "CRA 7 32 16")]
    [InlineData("ak 15 # 100-20 torre 2 apto 302", "AK 15 100 20 TO 2 AP 302")]
    public void Validate_ValidAddress_ReturnsNormalizedWithFullConfidence(string input, string expected)
    {
        var result = AddressValidator.Validate(input);

        result.Address.Should().NotBeNull();
        result.Normalized.Should().Be(expected);
        result.Confidence.Should().Be(1.0);
        result.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidAddress_ReturnsParsedAddress()
    {
        var result = AddressValidator.Validate("calle 59 # 80-12");

        result.Address.Should().Be(new Address(RoadType.Calle, new RoadNumber(59), new RoadNumber(80), 12));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Validate_BlankInput_ReturnsEmptyIssueWithZeroConfidence(string input)
    {
        var result = AddressValidator.Validate(input);

        result.Address.Should().BeNull();
        result.Normalized.Should().BeNull();
        result.Confidence.Should().Be(0.0);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.Empty);
    }

    [Theory]
    [InlineData("calle 59 # 80")]
    [InlineData("59 # 80-12")]
    [InlineData("hola mundo")]
    [InlineData("#-")]
    public void Validate_UnparseableInput_ReturnsUnparseableIssueWithZeroConfidence(string input)
    {
        var result = AddressValidator.Validate(input);

        result.Address.Should().BeNull();
        result.Normalized.Should().BeNull();
        result.Confidence.Should().Be(0.0);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.Unparseable);
    }

    [Theory]
    [InlineData("calle 59 # 80-12 apto 302 frente al parque", "CL 59 80 12 AP 302 FRENTE AL PARQUE")]
    [InlineData("calle 59 # 80-12 al lado de la iglesia", "CL 59 80 12 AL LADO DE LA IGLESIA")]
    public void Validate_TrailingUnrecognizedText_KeepsTextAtTheEndWithUnrecognizedTextIssue(
        string input, string expected)
    {
        var result = AddressValidator.Validate(input);

        result.Normalized.Should().Be(expected);
        result.Confidence.Should().Be(0.7);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.UnrecognizedText);
    }

    [Fact]
    public void Validate_TrailingUnrecognizedText_MentionsTextInMessage()
    {
        var result = AddressValidator.Validate("calle 59 # 80-12 frente al parque");

        result.Issues.Single().Message.Should().Contain("\"frente al parque\"");
    }

    [Theory]
    [InlineData("calle 59 # 80-12 apto", "CL 59 80 12 APTO", "\"apto\"")]
    [InlineData("calle 59 # 80-12 torre 2 apto", "CL 59 80 12 TO 2 APTO", "\"apto\"")]
    [InlineData("calle 59 # 80-12 torre azul", "CL 59 80 12 TORRE AZUL", "\"torre azul\"")]
    public void Validate_ComplementWithoutValue_ReturnsIncompleteComplementIssue(
        string input, string expected, string expectedDetail)
    {
        var result = AddressValidator.Validate(input);

        result.Normalized.Should().Be(expected);
        result.Confidence.Should().Be(0.7);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.IncompleteComplement);
        result.Issues.Single().Message.Should().Contain(expectedDetail);
    }

    [Fact]
    public void Validate_TrailingCity_ReturnsNoIssues()
    {
        var result = AddressValidator.Validate("Cra 7 # 10-20 apto 501, Bogotá D.C.");

        result.Normalized.Should().Be("CRA 7 10 20 AP 501");
        result.Confidence.Should().Be(1.0);
        result.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MisspelledRoadType_ReturnsCorrectedTypoIssue()
    {
        var result = AddressValidator.Validate("Carrerra 7 # 10-20");

        result.Normalized.Should().Be("CRA 7 10 20");
        result.Confidence.Should().Be(0.9);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.CorrectedTypo);
        result.Issues.Single().Message.Should().Contain("\"carrerra\"");
    }

    [Fact]
    public void Validate_NamedAutopista_ReturnsNamedRoadIssue()
    {
        var result = AddressValidator.Validate("Autopista Norte # 128-50");

        result.Normalized.Should().Be("AU NORTE 128 50");
        result.Confidence.Should().Be(0.8);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.NamedRoad);
    }

    [Fact]
    public void Validate_TrailingUnrecognizedText_StoresTextAsRemarks()
    {
        var result = AddressValidator.Validate("calle 59 # 80-12 frente al Párque");

        result.Address!.Remarks.Should().Be("FRENTE AL PARQUE");
    }

    [Fact]
    public void Validate_NoTrailingText_LeavesRemarksEmpty()
    {
        var result = AddressValidator.Validate("calle 59 # 80-12 apto 302");

        result.Address!.Remarks.Should().BeNull();
    }

    [Theory]
    [InlineData("km 5 via la calera vereda el salitre")]
    [InlineData("Vereda El Rosal, Finca La Esperanza")]
    [InlineData("Kilómetro 12 vía a Girardot")]
    [InlineData("Corregimiento Santa Elena, hacienda los alpes")]
    public void Validate_UnparseableRuralAddress_ReturnsRuralAndUnparseableIssues(string input)
    {
        var result = AddressValidator.Validate(input);

        result.Address.Should().BeNull();
        result.Confidence.Should().Be(0.0);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.RuralAddress, IssueCode.Unparseable);
    }

    [Fact]
    public void Validate_ParseableAddressWithRuralKeyword_FlagsRuralAddress()
    {
        var result = AddressValidator.Validate("calle 5 # 3-20 vereda el salitre");

        result.Normalized.Should().Be("CL 5 3 20 VEREDA EL SALITRE");
        result.Confidence.Should().Be(0.0);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.RuralAddress, IssueCode.UnrecognizedText);
    }

    [Theory]
    [InlineData("calle 0 # 80-12")]
    [InlineData("calle 59 # 0-12")]
    [InlineData("calle 59 # 80-0")]
    [InlineData("calle 59 # 80-00")]
    [InlineData("calle 0 # 0-0")]
    public void Validate_ZeroRoadCrossOrPlate_ReturnsSingleZeroNumberIssue(string input)
    {
        var result = AddressValidator.Validate(input);

        result.Address.Should().NotBeNull();
        result.Confidence.Should().Be(0.6);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.ZeroNumber);
    }

    [Theory]
    [InlineData("calle 59 # 80-251")]
    [InlineData("calle 59 # 80-850")]
    [InlineData("calle 59 # 80-9999")]
    public void Validate_PlateNumberAbove250_ReturnsPlateNumberOutOfRangeIssue(string input)
    {
        var result = AddressValidator.Validate(input);

        result.Address.Should().NotBeNull();
        result.Confidence.Should().Be(0.7);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.PlateNumberOutOfRange);
    }

    [Theory]
    [InlineData("calle 59 # 80-1")]
    [InlineData("calle 59 # 80-150")]
    [InlineData("cr 43a # 1 sur - 188")]
    [InlineData("calle 59 # 80-250")]
    [InlineData("calle 200 # 300-99")]
    public void Validate_PlateNumberUpTo250_ReturnsNoIssues(string input)
    {
        AddressValidator.Validate(input).Issues.Should().BeEmpty();
    }

    [Fact]
    public void Validate_SeveralIssues_AddsUpPenalties()
    {
        var result = AddressValidator.Validate("calle 0 # 80-851 frente al parque");

        result.Issues.Select(issue => issue.Code).Should().Equal(
            IssueCode.UnrecognizedText, IssueCode.ZeroNumber, IssueCode.PlateNumberOutOfRange);
        result.Confidence.Should().Be(0.0);
    }

    [Fact]
    public void Validate_AnyIssue_HasMessage()
    {
        var result = AddressValidator.Validate("calle 59 # 80");

        result.Issues.Should().OnlyContain(issue => !string.IsNullOrWhiteSpace(issue.Message));
    }

    [Fact]
    public void Validate_SameInputTwice_ReturnsEqualResults()
    {
        AddressValidator.Validate("calle 59 # 80").Should().Be(AddressValidator.Validate("calle 59 # 80"));
    }

    [Fact]
    public void Validate_UnknownNamedAvenue_ReturnsNamedRoadIssue()
    {
        var result = AddressValidator.Validate("av el poblado # 30-28");

        result.Normalized.Should().Be("AV EL POBLADO 30 28");
        result.Confidence.Should().Be(0.8);
        result.Issues.Select(issue => issue.Code).Should().Equal(IssueCode.NamedRoad);
    }

    [Fact]
    public void Validate_KnownNamedAvenue_ReturnsNoIssues()
    {
        var result = AddressValidator.Validate("Av el dorado # 30 28");

        result.Normalized.Should().Be("AC 26 30 28");
        result.Issues.Should().BeEmpty();
    }
}
