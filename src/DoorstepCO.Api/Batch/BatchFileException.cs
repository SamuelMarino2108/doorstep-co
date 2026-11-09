namespace DoorstepCO.Api.Batch;

/// <summary>
/// Thrown when an uploaded batch file cannot be processed.
/// </summary>
public sealed class BatchFileException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchFileException"/> class.
    /// </summary>
    /// <param name="error">Reason why the file cannot be processed.</param>
    public BatchFileException(BatchError error)
        : base(MessageFor(error))
    {
        Error = error;
    }

    /// <summary>
    /// Reason why the file cannot be processed.
    /// </summary>
    public BatchError Error { get; }

    private static string MessageFor(BatchError error) => error switch
    {
        BatchError.EmptyFile => "The file is empty.",
        BatchError.MissingAddressColumn => $"The file has no {BatchColumns.AddressHeader} column.",
        BatchError.UnreadableFile => "The file could not be read as a spreadsheet.",
        BatchError.UnsupportedFileType => "Only .csv and .xlsx files are supported.",
        BatchError.FileTooLarge => $"The file is larger than {BatchFile.MaxSizeInBytes / (1024 * 1024)} MB.",
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, null),
    };
}
