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
        ["k"] = RoadType.Carrera,

        ["diagonal"] = RoadType.Diagonal,
        ["diag"] = RoadType.Diagonal,
        ["dg"] = RoadType.Diagonal,
        ["dgn"] = RoadType.Diagonal,

        ["transversal"] = RoadType.Transversal,
        ["transv"] = RoadType.Transversal,
        ["trans"] = RoadType.Transversal,
        ["tv"] = RoadType.Transversal,
        ["tr"] = RoadType.Transversal,

        ["avenida"] = RoadType.Avenida,
        ["avda"] = RoadType.Avenida,
        ["av"] = RoadType.Avenida,
        ["ave"] = RoadType.Avenida,

        ["ac"] = RoadType.AvenidaCalle,
        ["ak"] = RoadType.AvenidaCarrera,

        ["autopista"] = RoadType.Autopista,
        ["autop"] = RoadType.Autopista,
        ["au"] = RoadType.Autopista,

        ["circular"] = RoadType.Circular,
        ["circ"] = RoadType.Circular,
        ["cq"] = RoadType.Circular,

        ["circunvalar"] = RoadType.Circunvalar,
        ["circunv"] = RoadType.Circunvalar,
        ["cv"] = RoadType.Circunvalar,
    };

    private static readonly HashSet<RoadType> _named =
    [
        RoadType.Avenida,
        RoadType.Autopista,
        RoadType.Circunvalar,
    ];

    private static readonly Dictionary<(RoadType First, RoadType Second), RoadType> _compounds = new()
    {
        [(RoadType.Avenida, RoadType.Calle)] = RoadType.AvenidaCalle,
        [(RoadType.Avenida, RoadType.Carrera)] = RoadType.AvenidaCarrera,
    };

    public static bool TryGetRoadType(string keyword, out RoadType roadType) =>
        _keywords.TryGetValue(keyword, out roadType);

    public static bool CanBeNamed(RoadType roadType) => _named.Contains(roadType);

    public static bool TryCombine(RoadType first, RoadType second, out RoadType compound) =>
        _compounds.TryGetValue((first, second), out compound);
}
