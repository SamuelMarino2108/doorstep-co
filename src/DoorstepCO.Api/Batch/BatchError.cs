namespace DoorstepCO.Api.Batch;

/// <summary>
/// Reason why an uploaded batch file cannot be processed.
/// </summary>
public enum BatchError
{
    /// <summary>The file has no content or no header row.</summary>
    EmptyFile,

    /// <summary>The header row has no DIRECCION column.</summary>
    MissingAddressColumn,

    /// <summary>The file could not be opened as a spreadsheet.</summary>
    UnreadableFile,
}
