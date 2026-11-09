using DoorstepCO.Core.Parsing;

namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Tokens that separate the road number from the cross number, e.g. "#" or "no" in "calle 59 no 80-12".
/// </summary>
internal static class PlateSeparatorKeywords
{
    private static readonly HashSet<string> _separators = new(StringComparer.Ordinal)
    {
        AddressTokenizer.NumberSign,
        "no",
        "nro",
        "n",
        "num",
        "numero",
    };

    public static bool IsPlateSeparator(string token) => _separators.Contains(token);
}
