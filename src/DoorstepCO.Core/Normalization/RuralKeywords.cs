namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Normalized keywords that mark a rural address, e.g. "vereda" in "vereda el salitre, finca la esperanza".
/// </summary>
internal static class RuralKeywords
{
    private static readonly HashSet<string> _keywords = new(StringComparer.Ordinal)
    {
        "vereda",
        "finca",
        "km",
        "kilometro",
        "corregimiento",
        "hacienda",
        "parcela",
    };

    public static bool IsRural(string token) => _keywords.Contains(token);
}
