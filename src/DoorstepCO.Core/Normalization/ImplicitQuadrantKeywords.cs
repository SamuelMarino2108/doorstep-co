namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Quadrants that are implicit in the standard nomenclature and are dropped when written, e.g. "norte" in
/// "Calle 100 Norte # 15-20": in Bogotá and Medellín addresses are north unless marked "Sur".
/// </summary>
internal static class ImplicitQuadrantKeywords
{
    private static readonly HashSet<string> _keywords = new(StringComparer.Ordinal)
    {
        "norte",
    };

    public static bool IsImplicitQuadrant(string token) => _keywords.Contains(token);
}
