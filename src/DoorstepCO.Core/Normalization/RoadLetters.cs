namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Single-letter suffixes of a road number, e.g. "a" in "calle 59 a". Letters that are reserved for other
/// keywords ("n" for "numero", "s" and "e" for quadrants) are never road letters.
/// </summary>
internal static class RoadLetters
{
    public static bool TryGetLetter(string token, out char letter)
    {
        letter = default;
        if (token.Length != 1
            || !char.IsAsciiLetterLower(token[0])
            || PlateSeparatorKeywords.IsPlateSeparator(token)
            || QuadrantKeywords.TryGetQuadrant(token, out _))
        {
            return false;
        }

        letter = char.ToUpperInvariant(token[0]);
        return true;
    }
}
