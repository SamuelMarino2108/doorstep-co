namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Single-letter suffixes of a road number, e.g. "a" in "calle 59 a". Letters that are reserved for other
/// keywords (such as "n" for "numero") are never road letters.
/// </summary>
internal static class RoadLetters
{
    public static bool TryGetLetter(string token, out char letter)
    {
        letter = default;
        if (token.Length != 1 || !char.IsAsciiLetterLower(token[0]) || PlateSeparatorKeywords.IsPlateSeparator(token))
        {
            return false;
        }

        letter = char.ToUpperInvariant(token[0]);
        return true;
    }
}
