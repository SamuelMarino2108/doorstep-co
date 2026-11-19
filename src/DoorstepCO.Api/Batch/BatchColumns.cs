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
    public const string ConfidenceHeader = "CONFIANZA";
    public const string IssuesHeader = "PROBLEMAS";
    public const string IssueSeparator = "; ";

    public static readonly IReadOnlyList<string> ResultHeaders = [NormalizedHeader, ConfidenceHeader, IssuesHeader];

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

    public static BatchRow Evaluate(string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return BatchRow.Blank;
        }

        var result = AddressValidator.Validate(address);
        return new BatchRow(
            result.Normalized ?? string.Empty,
            result.Confidence,
            string.Join(IssueSeparator, result.Issues.Select(issue => issue.Message)));
    }

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
