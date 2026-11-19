using System.Text;
using DoorstepCO.Api.Batch;

namespace DoorstepCO.Api.Tests.Batch;

public sealed class CsvBatchTests
{
    private static readonly Encoding _utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    [Fact]
    public void Normalize_ValidFile_AppendsNormalizedConfidenceAndIssuesColumns()
    {
        var input = "ID;DIRECCION;CIUDAD\r\n1;cll 59 a bis sur # 80-12;Bogotá\r\n2;frente al parque;Cali\r\n";

        var output = Decode(CsvBatch.Normalize(Encoding.UTF8.GetBytes(input)));

        output.Should().Be(
            "ID;DIRECCION;CIUDAD;NORMALIZADA;CONFIANZA;PROBLEMAS\r\n" +
            "1;cll 59 a bis sur # 80-12;Bogotá;CL 59A BIS SUR 80 12;1.00;\r\n" +
            "2;frente al parque;Cali;;0.00;The address could not be recognized as an urban address.\r\n");
    }

    [Fact]
    public void Normalize_AddressWithSeveralIssues_JoinsIssueMessagesInOneQuotedCell()
    {
        var input = "DIRECCION;ID\ncll 0 # 2-3 frente al parque;1\n";

        var output = Decode(CsvBatch.Normalize(Encoding.UTF8.GetBytes(input)));

        output.Should().EndWith(
            ";1;CL 0 2 3 FRENTE AL PARQUE;0.30;\"" +
            "Text after the address was not recognized and was kept as remarks. \"\"frente al parque\"\"; " +
            "The road, crossing road or plate number is 0.\"\r\n");
    }

    [Theory]
    [InlineData("Direccion")]
    [InlineData("dirección")]
    [InlineData("DIRECCIÓN")]
    [InlineData(" DIRECCION ")]
    public void Normalize_HeaderVariants_FindsAddressColumn(string header)
    {
        var input = $"{header}\ncra 7 # 10-20\n";

        var output = Decode(CsvBatch.Normalize(Encoding.UTF8.GetBytes(input)));

        output.Should().EndWith("CRA 7 10 20,1.00,\r\n");
    }

    [Fact]
    public void Normalize_Windows1252File_DecodesAccents()
    {
        var windows1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;
        var input = windows1252.GetBytes("DIRECCIÓN,BARRIO\ncll 1 # 2-3,Nariño\n");

        var output = Decode(CsvBatch.Normalize(input));

        output.Should().Contain("DIRECCIÓN").And.Contain("Nariño");
    }

    [Fact]
    public void Normalize_AnyFile_WritesUtf8WithBom()
    {
        var output = CsvBatch.Normalize(Encoding.UTF8.GetBytes("DIRECCION\ncll 1 # 2-3\n"));

        output.Take(3).Should().Equal(_utf8WithBom.GetPreamble());
    }

    [Fact]
    public void Normalize_ShortRowsAndBlankLines_KeepsRowsAligned()
    {
        var input = "ID,DIRECCION\n1\n\n2,cll 1 # 2-3\n";

        var output = Decode(CsvBatch.Normalize(Encoding.UTF8.GetBytes(input)));

        output.Should().Be(
            "ID,DIRECCION,NORMALIZADA,CONFIANZA,PROBLEMAS\r\n1,,,,\r\n\r\n2,cll 1 # 2-3,CL 1 2 3,1.00,\r\n");
    }

    [Fact]
    public void Normalize_MissingAddressColumn_ThrowsBatchFileException()
    {
        var act = () => CsvBatch.Normalize(Encoding.UTF8.GetBytes("ID,CIUDAD\n1,Cali\n"));

        act.Should().Throw<BatchFileException>().Which.Error.Should().Be(BatchError.MissingAddressColumn);
    }

    [Fact]
    public void Normalize_EmptyFile_ThrowsBatchFileException()
    {
        var act = () => CsvBatch.Normalize([]);

        act.Should().Throw<BatchFileException>().Which.Error.Should().Be(BatchError.EmptyFile);
    }

    private static string Decode(byte[] bytes) => _utf8WithBom.GetString(bytes).TrimStart('\uFEFF');
}
