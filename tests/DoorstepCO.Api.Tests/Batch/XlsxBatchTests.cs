using ClosedXML.Excel;
using DoorstepCO.Api.Batch;

namespace DoorstepCO.Api.Tests.Batch;

public sealed class XlsxBatchTests
{
    [Fact]
    public void Normalize_ValidFile_AppendsNormalizedColumn()
    {
        var input = CreateWorkbook(
            ["ID", "DIRECCION", "CIUDAD"],
            [1, "cll 59 a bis sur # 80-12", "Bogotá"],
            [2, "frente al parque", "Cali"]);

        using var output = Open(XlsxBatch.Normalize(input));

        var sheet = output.Worksheet(1);
        sheet.Cell(1, 4).GetString().Should().Be("NORMALIZADA");
        sheet.Cell(2, 4).GetString().Should().Be("CL 59A BIS SUR 80 12");
        sheet.Cell(3, 4).IsEmpty().Should().BeTrue();
        sheet.Cell(2, 1).GetValue<int>().Should().Be(1);
        sheet.Cell(2, 3).GetString().Should().Be("Bogotá");
    }

    [Theory]
    [InlineData("Direccion")]
    [InlineData("dirección")]
    [InlineData("DIRECCIÓN")]
    public void Normalize_HeaderVariants_FindsAddressColumn(string header)
    {
        var input = CreateWorkbook([header], ["cra 7 # 10-20"]);

        using var output = Open(XlsxBatch.Normalize(input));

        output.Worksheet(1).Cell(2, 2).GetString().Should().Be("CRA 7 10 20");
    }

    [Fact]
    public void Normalize_HeaderRowStyle_CopiedToNewHeader()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Pedidos");
        sheet.Cell(1, 1).Value = "DIRECCION";
        sheet.Cell(1, 1).Style.Font.Bold = true;
        sheet.Cell(2, 1).Value = "cll 1 # 2-3";

        using var output = Open(XlsxBatch.Normalize(Save(workbook)));

        output.Worksheet(1).Cell(1, 2).Style.Font.Bold.Should().BeTrue();
        output.Worksheet(1).Name.Should().Be("Pedidos");
    }

    [Fact]
    public void Normalize_EmptyAddressCells_LeavesNormalizedCellEmpty()
    {
        var input = CreateWorkbook(["DIRECCION", "ID"], ["", 1], ["cll 1 # 2-3", 2]);

        using var output = Open(XlsxBatch.Normalize(input));

        var sheet = output.Worksheet(1);
        sheet.Cell(2, 3).IsEmpty().Should().BeTrue();
        sheet.Cell(3, 3).GetString().Should().Be("CL 1 2 3");
    }

    [Fact]
    public void Normalize_MissingAddressColumn_ThrowsBatchFileException()
    {
        var input = CreateWorkbook(["ID", "CIUDAD"], [1, "Cali"]);

        var act = () => XlsxBatch.Normalize(input);

        act.Should().Throw<BatchFileException>().Which.Error.Should().Be(BatchError.MissingAddressColumn);
    }

    [Fact]
    public void Normalize_EmptySheet_ThrowsBatchFileException()
    {
        using var workbook = new XLWorkbook();
        workbook.AddWorksheet("Hoja1");

        var act = () => XlsxBatch.Normalize(Save(workbook));

        act.Should().Throw<BatchFileException>().Which.Error.Should().Be(BatchError.EmptyFile);
    }

    [Fact]
    public void Normalize_NotAnXlsxFile_ThrowsBatchFileException()
    {
        var act = () => XlsxBatch.Normalize("DIRECCION\ncll 1 # 2-3\n"u8.ToArray());

        act.Should().Throw<BatchFileException>().Which.Error.Should().Be(BatchError.UnreadableFile);
    }

    private static byte[] CreateWorkbook(params XLCellValue[][] rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Hoja1");
        for (var row = 0; row < rows.Length; row++)
        {
            for (var column = 0; column < rows[row].Length; column++)
            {
                sheet.Cell(row + 1, column + 1).Value = rows[row][column];
            }
        }

        return Save(workbook);
    }

    private static byte[] Save(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static XLWorkbook Open(byte[] content) => new(new MemoryStream(content));
}
