namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Ordinal forms of a road number: suffixes written after the digits ("ra" in "calle 1ra", "da" in "calle 2da") and
/// spelled-out ordinals ("calle primera").
/// </summary>
internal static class OrdinalKeywords
{
    private static readonly HashSet<string> _suffixes = new(StringComparer.Ordinal)
    {
        "ra",
        "ro",
        "era",
        "ero",
        "er",
        "da",
        "do",
        "ta",
        "to",
    };

    private static readonly Dictionary<string, int> _words = new(StringComparer.Ordinal)
    {
        ["primera"] = 1,
        ["primero"] = 1,
        ["segunda"] = 2,
        ["segundo"] = 2,
        ["tercera"] = 3,
        ["tercero"] = 3,
        ["cuarta"] = 4,
        ["cuarto"] = 4,
        ["quinta"] = 5,
        ["quinto"] = 5,
        ["sexta"] = 6,
        ["sexto"] = 6,
        ["septima"] = 7,
        ["septimo"] = 7,
        ["octava"] = 8,
        ["octavo"] = 8,
        ["novena"] = 9,
        ["noveno"] = 9,
        ["decima"] = 10,
        ["decimo"] = 10,
    };

    public static bool IsOrdinalSuffix(string token) => _suffixes.Contains(token);

    public static bool TryGetNumber(string word, out int number) => _words.TryGetValue(word, out number);
}
