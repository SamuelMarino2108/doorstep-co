using System.Text;
using DoorstepCO.Api.Batch;

namespace DoorstepCO.Api.Tests.Batch;

public sealed class CsvBatchTests
{
    private static readonly Encoding _utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    [Fact]
    public void Normalize_ValidFile_AppendsNormalizedColumn()
    {
        var input = "ID;DIRECCION;CIUDAD\r\n1;cll 59 a bis sur # 80-12;Bogotá\r\n2;frente al parque;Cali\r\n";

        var output = Decode(CsvBatch.Normalize(Encoding.UTF8.GetBytes(input)));

        output.Should().Be(
            "ID;DIRECCION;CIUDAD;NORMALIZADA\r\n" +
            "1;cll 59 a bis sur # 80-12;Bogotá;CL 59A BIS SUR 80 12\r\n" +
            "2;frente al parque;Cali;\r\n");
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

        output.Should().EndWith("CRA 7 10 20\r\n");
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

        output.Should().Be("ID,DIRECCION,NORMALIZADA\r\n1,,\r\n\r\n2,cll 1 # 2-3,CL 1 2 3\r\n");
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
