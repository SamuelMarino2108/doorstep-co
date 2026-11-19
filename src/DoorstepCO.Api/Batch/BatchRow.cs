namespace DoorstepCO.Api.Batch;

/// <summary>
/// Result cells appended to a row of a batch file.
/// </summary>
/// <param name="Normalized">Normalized address, or empty when it could not be parsed or was blank.</param>
/// <param name="Confidence">Confidence score, or <see langword="null"/> when the address was blank.</param>
/// <param name="Issues">Issue messages joined by <see cref="BatchColumns.IssueSeparator"/>, or empty.</param>
internal sealed record BatchRow(string Normalized, double? Confidence, string Issues)
{
    public static readonly BatchRow Blank = new(string.Empty, null, string.Empty);
}
