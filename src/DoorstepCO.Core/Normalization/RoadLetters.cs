using System.Diagnostics.CodeAnalysis;

namespace DoorstepCO.Core.Normalization;

/// <summary>
/// One- or two-letter suffixes of a road number, e.g. "a" in "calle 59 a" or "ae" in "calle 44ae" (Medellín).
/// Tokens that are reserved for other keywords ("n" or "no" for "numero", "s" and "e" for quadrants) are never
/// road letters.
/// </summary>
internal static class RoadLetters
{
    private const int MaxLetters = 2;

    public static bool TryGetLetter(string token, [NotNullWhen(true)] out string? letter)
    {
        letter = null;
        if (token.Length is 0 or > MaxLetters
            || !token.All(char.IsAsciiLetterLower)
            || PlateSeparatorKeywords.IsPlateSeparator(token)
            || QuadrantKeywords.TryGetQuadrant(token, out _))
        {
            return false;
        }

        letter = token.ToUpperInvariant();
        return true;
    }
}
