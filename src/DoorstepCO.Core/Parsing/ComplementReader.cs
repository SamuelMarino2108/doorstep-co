using System.Diagnostics.CodeAnalysis;
using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Parsing;

/// <summary>
/// Reads the complements that follow the plate number, e.g. "torre 2 apto 302 b". Each complement is a keyword
/// followed by a value: a number with an optional letter suffix, or a single letter. The value may be preceded by
/// a plate separator or a dash ("apto #302", "ap-302"), and complements may be separated by a dash
/// ("apto 302 - torre 4"). A letter after the number
/// that is itself a complement keyword ("t" in "apto 302 t 2") starts the next complement instead.
/// A conjunto, edificio, barrio or urbanización takes a name instead: every word up to the next complement
/// keyword or trailing city ("los pinos" in "conjunto los pinos torre 2", "chapinero" in "barrio chapinero bogota").
/// Reading stops at the first token that does not start a complete complement; the caller decides what to do with
/// the rest.
/// </summary>
internal static class ComplementReader
{
    public static int ReadValid(IReadOnlyList<string> tokens, int position, out IReadOnlyList<Complement> complements)
    {
        var read = new List<Complement>();
        while (position < tokens.Count)
        {
            var start = position;
            if (read.Count > 0 && tokens[position] == AddressTokenizer.Dash)
            {
                position++;
            }

            if (position >= tokens.Count || !TryReadOne(tokens, ref position, out var complement))
            {
                position = start;
                break;
            }

            read.Add(complement);
        }

        complements = read.ToArray();
        return position;
    }

    private static bool TryReadOne(
        IReadOnlyList<string> tokens, ref int position, [NotNullWhen(true)] out Complement? complement)
    {
        complement = null;
        if (!ComplementKeywords.TryGetComplementType(tokens[position], out var type))
        {
            return false;
        }

        position++;
        if (type == ComplementType.Conjunto
            && position < tokens.Count
            && ComplementKeywords.IsResidential(tokens[position]))
        {
            type = ComplementType.ConjuntoResidencial;
            position++;
        }

        var hasValue = ComplementKeywords.TakesName(type)
            ? TryReadName(tokens, ref position, out var value)
            : TryReadValue(tokens, ref position, out value);
        if (!hasValue)
        {
            return false;
        }

        complement = new Complement(type, value);
        return true;
    }

    private static bool TryReadValue(IReadOnlyList<string> tokens, ref int position, out string value)
    {
        value = string.Empty;
        if (position >= tokens.Count)
        {
            return false;
        }

        if (IsValueSeparator(tokens[position]) && position + 1 < tokens.Count)
        {
            position++;
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

    private static bool TryReadName(IReadOnlyList<string> tokens, ref int position, out string name)
    {
        var words = new List<string>();
        while (position < tokens.Count
            && !ComplementKeywords.TryGetComplementType(tokens[position], out _)
            && IsWord(tokens[position])
            && !(words.Count > 0 && LocationKeywords.IsLocationSuffix(tokens, position)))
        {
            words.Add(tokens[position].ToUpperInvariant());
            position++;
        }

        name = string.Join(' ', words);
        return words.Count > 0;
    }

    private static bool IsValueSeparator(string token) =>
        token == AddressTokenizer.Dash || PlateSeparatorKeywords.IsPlateSeparator(token);

    private static bool IsWord(string token) => token.Length > 0 && token.All(char.IsAsciiLetterOrDigit);

    private static bool IsLetter(string token) => token.Length == 1 && char.IsAsciiLetterLower(token[0]);

    private static bool IsNumber(string token) => token.Length > 0 && token.All(char.IsAsciiDigit);
}
