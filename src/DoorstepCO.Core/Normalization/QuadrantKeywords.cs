namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Maps normalized quadrant keywords to <see cref="Quadrant"/>. The single letters "s" and "e" are
/// quadrants, never road letters.
/// </summary>
internal static class QuadrantKeywords
{
    private static readonly Dictionary<string, Quadrant> _keywords = new(StringComparer.Ordinal)
    {
        ["sur"] = Quadrant.Sur,
        ["s"] = Quadrant.Sur,

        ["este"] = Quadrant.Este,
        ["e"] = Quadrant.Este,
    };

    public static bool TryGetQuadrant(string keyword, out Quadrant quadrant) =>
        _keywords.TryGetValue(keyword, out quadrant);
}
