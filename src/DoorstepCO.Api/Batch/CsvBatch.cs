using System.Text;

namespace DoorstepCO.Api.Batch;

/// <summary>
/// Normalizes the addresses of a CSV file.
/// </summary>
internal static class CsvBatch
{
    private const int Windows1252CodePage = 1252;

    private static readonly UTF8Encoding _strictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly UTF8Encoding _utf8WithBom = new(encoderShouldEmitUTF8Identifier: true);

    /// <summary>
    /// Returns the file with a NORMALIZADA column appended, encoded as UTF-8 with BOM so Excel shows accents
    /// correctly. The delimiter of the original file is kept.
    /// </summary>
    public static byte[] Normalize(byte[] content)
    {
        var text = Decode(content);
        var delimiter = CsvFormat.DetectDelimiter(text);
        var rows = CsvFormat.Parse(text, delimiter);
        if (rows.Count == 0)
        {
            throw new BatchFileException(BatchError.EmptyFile);
        }

        var header = rows[0];
        var addressColumn = BatchColumns.FindAddressColumn(header);
        if (addressColumn < 0)
        {
            throw new BatchFileException(BatchError.MissingAddressColumn);
        }

        List<IReadOnlyList<string>> output = [[.. header, BatchColumns.NormalizedHeader]];
        foreach (var row in rows.Skip(1))
        {
            output.Add(IsBlank(row) ? row : NormalizeRow(row, header.Count, addressColumn));
        }

        var result = CsvFormat.Write(output, delimiter);
        return [.. _utf8WithBom.GetPreamble(), .. _utf8WithBom.GetBytes(result)];
    }

    private static string[] NormalizeRow(IReadOnlyList<string> row, int headerCount, int addressColumn)
    {
        var cells = new string[Math.Max(row.Count, headerCount) + 1];
        for (var i = 0; i < cells.Length - 1; i++)
        {
            cells[i] = i < row.Count ? row[i] : string.Empty;
        }

        cells[^1] = BatchColumns.NormalizedValue(cells[addressColumn]);
        return cells;
    }

    private static bool IsBlank(IReadOnlyList<string> row) => row.Count == 1 && row[0].Length == 0;

    private static string Decode(byte[] content)
    {
        var preamble = _utf8WithBom.Preamble;
        if (content.AsSpan().StartsWith(preamble))
        {
            return _utf8WithBom.GetString(content, preamble.Length, content.Length - preamble.Length);
        }

        try
        {
            return _strictUtf8.GetString(content);
        }
        catch (DecoderFallbackException)
        {
            return CodePagesEncodingProvider.Instance.GetEncoding(Windows1252CodePage)!.GetString(content);
        }
    }
}
