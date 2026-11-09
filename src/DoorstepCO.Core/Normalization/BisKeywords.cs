namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Normalized keywords for the "bis" modifier, e.g. "bis" in "calle 59 bis # 80-12".
/// </summary>
internal static class BisKeywords
{
    private static readonly HashSet<string> _keywords = new(StringComparer.Ordinal)
    {
        "bis",
    };

    public static bool IsBis(string token) => _keywords.Contains(token);
}
