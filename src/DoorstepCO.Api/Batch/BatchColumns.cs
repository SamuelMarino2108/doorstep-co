using System.Globalization;
using System.Text;
using DoorstepCO.Core;

namespace DoorstepCO.Api.Batch;

/// <summary>
/// Locates the address column of a batch file and computes the value of the column added to it.
/// </summary>
internal static class BatchColumns
{
    public const string AddressHeader = "DIRECCION";
    public const string NormalizedHeader = "NORMALIZADA";

    public static int FindAddressColumn(IReadOnlyList<string> headers)
    {
        for (var i = 0; i < headers.Count; i++)
        {
            if (string.Equals(RemoveAccents(headers[i].Trim()), AddressHeader, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    public static string NormalizedValue(string? address) =>
        string.IsNullOrWhiteSpace(address) ? string.Empty : AddressValidator.Validate(address).Normalized ?? string.Empty;

    private static string RemoveAccents(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var c in text.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
