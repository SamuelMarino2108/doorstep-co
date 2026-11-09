using System.Globalization;
using System.Text;

namespace DoorstepCO.Core.Parsing;

/// <summary>
/// Splits a raw address into lowercase, accent-free tokens: runs of letters, runs of digits,
/// and the symbols <see cref="NumberSign"/> and <see cref="Dash"/>. Any other character separates tokens.
/// </summary>
internal static class AddressTokenizer
{
    public const string NumberSign = "#";
    public const string Dash = "-";

    private const char DegreeSign = '°';
    private const char OrdinalIndicator = 'º';

    public static IReadOnlyList<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();
        var currentKind = CharKind.Separator;

        foreach (var c in Normalize(input))
        {
            var kind = Classify(c);
            if (kind != currentKind || kind == CharKind.Symbol)
            {
                Flush(current, tokens);
                currentKind = kind;
            }

            if (kind != CharKind.Separator)
            {
                current.Append(c);
            }
        }

        Flush(current, tokens);
        return tokens;
    }

    private static string Normalize(string input)
    {
        var decomposed = input
            .Replace(DegreeSign, 'o')
            .Replace(OrdinalIndicator, 'o')
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    private static CharKind Classify(char c) => c switch
    {
        _ when char.IsAsciiLetter(c) => CharKind.Letter,
        _ when char.IsAsciiDigit(c) => CharKind.Digit,
        _ when c == NumberSign[0] || c == Dash[0] => CharKind.Symbol,
        _ => CharKind.Separator,
    };

    private static void Flush(StringBuilder current, List<string> tokens)
    {
        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
            current.Clear();
        }
    }

    private enum CharKind
    {
        Separator,
        Letter,
        Digit,
        Symbol,
    }
}
