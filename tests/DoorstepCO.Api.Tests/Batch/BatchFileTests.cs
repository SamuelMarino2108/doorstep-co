using System.Text;
using ClosedXML.Excel;
using DoorstepCO.Api.Batch;

namespace DoorstepCO.Api.Tests.Batch;

public sealed class BatchFileTests
{
    [Theory]
    [InlineData("pedidos.csv", "pedidos-normalizada.csv")]
    [InlineData("Pedidos.CSV", "Pedidos-normalizada.CSV")]
    public void Normalize_CsvFile_ReturnsCsvWithSuffixedName(string fileName, string expectedName)
    {
        var result = BatchFile.Normalize(fileName, Encoding.UTF8.GetBytes("DIRECCION\ncll 1 # 2-3\n"));

        result.FileName.Should().Be(expectedName);
        result.ContentType.Should().Be("text/csv");
        Encoding.UTF8.GetString(result.Content).Should().Contain("CL 1 2 3");
    }

    [Fact]
    public void Normalize_XlsxFile_ReturnsXlsxWithSuffixedName()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Hoja1");
        sheet.Cell(1, 1).Value = "DIRECCION";
        sheet.Cell(2, 1).Value = "cll 1 # 2-3";
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var result = BatchFile.Normalize("pedidos.xlsx", stream.ToArray());

        result.FileName.Should().Be("pedidos-normalizada.xlsx");
        result.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Theory]
    [InlineData("pedidos.xls")]
    [InlineData("pedidos.txt")]
    [InlineData("pedidos")]
    public void Normalize_UnsupportedExtension_ThrowsBatchFileException(string fileName)
    {
        var act = () => BatchFile.Normalize(fileName, [1, 2, 3]);

        act.Should().Throw<BatchFileException>().Which.Error.Should().Be(BatchError.UnsupportedFileType);
    }
}
