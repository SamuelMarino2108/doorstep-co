namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Maps normalized road type keywords (lowercase, no accents, no dots) to <see cref="RoadType"/>, and pairs
/// of road types written together (such as "avenida calle") to their compound road type.
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

    private static readonly Dictionary<(RoadType First, RoadType Second), RoadType> _compounds = new()
    {
        [(RoadType.Avenida, RoadType.Calle)] = RoadType.AvenidaCalle,
        [(RoadType.Avenida, RoadType.Carrera)] = RoadType.AvenidaCarrera,
    };

    public static bool TryGetRoadType(string keyword, out RoadType roadType) =>
        _keywords.TryGetValue(keyword, out roadType);

    public static bool TryCombine(RoadType first, RoadType second, out RoadType compound) =>
        _compounds.TryGetValue((first, second), out compound);
}
