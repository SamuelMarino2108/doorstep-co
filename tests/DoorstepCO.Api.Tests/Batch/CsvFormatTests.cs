using DoorstepCO.Api.Batch;

namespace DoorstepCO.Api.Tests.Batch;

public sealed class CsvFormatTests
{
    [Theory]
    [InlineData("ID;DIRECCION;CIUDAD", ';')]
    [InlineData("ID,DIRECCION,CIUDAD", ',')]
    [InlineData("ID\tDIRECCION\tCIUDAD", '\t')]
    [InlineData("DIRECCION", ',')]
    [InlineData("\"A;B\",C,D", ',')]
    public void DetectDelimiter_HeaderLine_ReturnsMostFrequentDelimiter(string text, char expected)
    {
        CsvFormat.DetectDelimiter(text).Should().Be(expected);
    }

    [Fact]
    public void Parse_SimpleRows_SplitsFieldsAndLines()
    {
        var rows = CsvFormat.Parse("a;b\r\nc;d\r\n", ';');

        rows.Should().HaveCount(2);
        rows[0].Should().Equal("a", "b");
        rows[1].Should().Equal("c", "d");
    }

    [Fact]
    public void Parse_QuotedFields_KeepsDelimitersQuotesAndNewLines()
    {
        var rows = CsvFormat.Parse("\"cll 59, sur\",\"dice \"\"hola\"\"\",\"línea\nnueva\"\n", ',');

        rows.Should().ContainSingle()
            .Which.Should().Equal("cll 59, sur", "dice \"hola\"", "línea\nnueva");
    }

    [Fact]
    public void Parse_NoTrailingNewLine_ReadsLastRow()
    {
        var rows = CsvFormat.Parse("a,b\nc,", ',');

        rows.Should().HaveCount(2);
        rows[1].Should().Equal("c", "");
    }

    [Fact]
    public void Parse_EmptyText_ReturnsNoRows()
    {
        CsvFormat.Parse("", ',').Should().BeEmpty();
    }

    [Fact]
    public void Write_FieldsNeedingQuotes_QuotesAndEscapesThem()
    {
        var text = CsvFormat.Write([["a;b", "dice \"hola\"", "simple", "x\ny"]], ';');

        text.Should().Be("\"a;b\";\"dice \"\"hola\"\"\";simple;\"x\ny\"\r\n");
    }
}
