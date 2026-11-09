using System.Diagnostics.CodeAnalysis;
using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Parsing;

/// <summary>
/// Reads the complements that follow the plate number, e.g. "torre 2 apto 302 b". Each complement is a keyword
/// followed by a value: a number with an optional letter suffix, or a single letter. A letter after the number
/// that is itself a complement keyword ("t" in "apto 302 t 2") starts the next complement instead.
/// </summary>
internal static class ComplementReader
{
    public static bool TryReadAll(
        IReadOnlyList<string> tokens, int position, [NotNullWhen(true)] out IReadOnlyList<Complement>? complements)
    {
        complements = null;
        var read = new List<Complement>();
        while (position < tokens.Count)
        {
            if (!ComplementKeywords.TryGetComplementType(tokens[position], out var type))
            {
                return false;
            }

            position++;
            if (!TryReadValue(tokens, ref position, out var value))
            {
                return false;
            }

            read.Add(new Complement(type, value));
        }

        complements = read.ToArray();
        return true;
    }

    private static bool TryReadValue(IReadOnlyList<string> tokens, ref int position, out string value)
    {
        value = string.Empty;
        if (position >= tokens.Count)
        {
            return false;
        }

        var token = tokens[position];
        if (IsLetter(token))
        {
            value = token.ToUpperInvariant();
            position++;
            return true;
        }

        if (!IsNumber(token))
        {
            return false;
        }

        value = token;
        position++;
        if (position < tokens.Count
            && IsLetter(tokens[position])
            && !ComplementKeywords.TryGetComplementType(tokens[position], out _))
        {
            value += tokens[position].ToUpperInvariant();
            position++;
        }

        return true;
    }

    private static bool IsLetter(string token) => token.Length == 1 && char.IsAsciiLetterLower(token[0]);

    private static bool IsNumber(string token) => token.Length > 0 && token.All(char.IsAsciiDigit);
}
