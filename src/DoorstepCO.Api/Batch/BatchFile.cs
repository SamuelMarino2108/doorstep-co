namespace DoorstepCO.Api.Batch;

/// <summary>
/// Normalizes the addresses of an uploaded file, choosing the format from its extension.
/// </summary>
internal static class BatchFile
{
    public const int MaxSizeInBytes = 10 * 1024 * 1024;

    private const string CsvExtension = ".csv";
    private const string XlsxExtension = ".xlsx";
    private const string CsvContentType = "text/csv";
    private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string OutputSuffix = "-normalizada";

    public static NormalizedFile Normalize(string fileName, byte[] content)
    {
        var extension = Path.GetExtension(fileName);
        var outputName = Path.GetFileNameWithoutExtension(fileName) + OutputSuffix + extension;

        if (string.Equals(extension, CsvExtension, StringComparison.OrdinalIgnoreCase))
        {
            return new NormalizedFile(CsvBatch.Normalize(content), CsvContentType, outputName);
        }

        if (string.Equals(extension, XlsxExtension, StringComparison.OrdinalIgnoreCase))
        {
            return new NormalizedFile(XlsxBatch.Normalize(content), XlsxContentType, outputName);
        }

        throw new BatchFileException(BatchError.UnsupportedFileType);
    }
}
