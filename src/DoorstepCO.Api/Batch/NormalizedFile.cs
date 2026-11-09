namespace DoorstepCO.Api.Batch;

/// <summary>
/// A batch file with its addresses normalized, ready to be downloaded.
/// </summary>
/// <param name="Content">Bytes of the file.</param>
/// <param name="ContentType">Media type of the file.</param>
/// <param name="FileName">Suggested name for the download.</param>
internal sealed record NormalizedFile(byte[] Content, string ContentType, string FileName);
