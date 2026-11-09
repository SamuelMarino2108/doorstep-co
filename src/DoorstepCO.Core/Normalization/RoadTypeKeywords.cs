namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Maps normalized road type keywords (lowercase, no accents, no dots) to <see cref="RoadType"/>.
/// </summary>
internal static class RoadTypeKeywords
{
    private static readonly Dictionary<string, RoadType> _keywords = new(StringComparer.Ordinal)
    {
        ["calle"] = RoadType.Calle,
        ["cll"] = RoadType.Calle,
        ["cl"] = RoadType.Calle,
        ["clle"] = RoadType.Calle,

        ["carrera"] = RoadType.Carrera,
        ["cra"] = RoadType.Carrera,
        ["kra"] = RoadType.Carrera,
        ["kr"] = RoadType.Carrera,
        ["cr"] = RoadType.Carrera,
        ["carr"] = RoadType.Carrera,

        ["diagonal"] = RoadType.Diagonal,
        ["diag"] = RoadType.Diagonal,
        ["dg"] = RoadType.Diagonal,

        ["transversal"] = RoadType.Transversal,
        ["transv"] = RoadType.Transversal,
        ["trans"] = RoadType.Transversal,
        ["tv"] = RoadType.Transversal,
        ["tr"] = RoadType.Transversal,

        ["avenida"] = RoadType.Avenida,
        ["avda"] = RoadType.Avenida,
        ["av"] = RoadType.Avenida,

        ["ac"] = RoadType.AvenidaCalle,
        ["ak"] = RoadType.AvenidaCarrera,
    };

    public static bool TryGetRoadType(string keyword, out RoadType roadType) =>
        _keywords.TryGetValue(keyword, out roadType);
}
