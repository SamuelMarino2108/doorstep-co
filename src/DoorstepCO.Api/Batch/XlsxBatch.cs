using ClosedXML.Excel;

namespace DoorstepCO.Api.Batch;

/// <summary>
/// Normalizes the addresses of the first worksheet of an Excel (.xlsx) file.
/// </summary>
internal static class XlsxBatch
{
    /// <summary>
    /// Returns the workbook with a NORMALIZADA column appended to the right of the used range of its first
    /// worksheet. The first non-empty row is the header row; everything else in the workbook is kept.
    /// </summary>
    public static byte[] Normalize(byte[] content)
    {
        using var workbook = Open(content);
        var sheet = workbook.Worksheet(1);
        var headerRow = sheet.FirstRowUsed() ?? throw new BatchFileException(BatchError.EmptyFile);

        var headerCells = headerRow.CellsUsed().ToList();
        var addressIndex = BatchColumns.FindAddressColumn([.. headerCells.Select(cell => cell.GetFormattedString())]);
        if (addressIndex < 0)
        {
            throw new BatchFileException(BatchError.MissingAddressColumn);
        }

        var addressColumn = headerCells[addressIndex].Address.ColumnNumber;
        var normalizedColumn = sheet.LastColumnUsed()!.ColumnNumber() + 1;
        var normalizedHeader = headerRow.Cell(normalizedColumn);
        normalizedHeader.Value = BatchColumns.NormalizedHeader;
        normalizedHeader.Style = headerCells[^1].Style;

        var lastRow = sheet.LastRowUsed()!.RowNumber();
        for (var row = headerRow.RowNumber() + 1; row <= lastRow; row++)
        {
            var normalized = BatchColumns.NormalizedValue(sheet.Cell(row, addressColumn).GetFormattedString());
            if (normalized.Length > 0)
            {
                sheet.Cell(row, normalizedColumn).Value = normalized;
            }
        }

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    private static XLWorkbook Open(byte[] content)
    {
        try
        {
            return new XLWorkbook(new MemoryStream(content));
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            throw new BatchFileException(BatchError.UnreadableFile);
        }
    }
}
