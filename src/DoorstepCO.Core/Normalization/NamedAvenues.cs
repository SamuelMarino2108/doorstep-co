using System.Diagnostics.CodeAnalysis;

namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Maps the normalized names of Bogotá avenues (lowercase, no accents, words separated by single spaces) to
/// their standard road, e.g. "el dorado" to Avenida Calle 26. Avenues whose number changes along their way,
/// such as Las Américas or Suba, are left out on purpose.
/// </summary>
internal static class NamedAvenues
{
    private static readonly Dictionary<string, (RoadType RoadType, RoadNumber Road)> _avenues =
        new(StringComparer.Ordinal)
        {
            ["el dorado"] = (RoadType.AvenidaCalle, new RoadNumber(26)),
            ["eldorado"] = (RoadType.AvenidaCalle, new RoadNumber(26)),
            ["jorge eliecer gaitan"] = (RoadType.AvenidaCalle, new RoadNumber(26)),

            ["caracas"] = (RoadType.AvenidaCarrera, new RoadNumber(14)),

            ["boyaca"] = (RoadType.AvenidaCarrera, new RoadNumber(72)),

            ["nqs"] = (RoadType.AvenidaCarrera, new RoadNumber(30)),
            ["norte quito sur"] = (RoadType.AvenidaCarrera, new RoadNumber(30)),
            ["ciudad de quito"] = (RoadType.AvenidaCarrera, new RoadNumber(30)),

            ["chile"] = (RoadType.AvenidaCalle, new RoadNumber(72)),

            ["primero de mayo"] = (RoadType.AvenidaCalle, new RoadNumber(22, Quadrant: Quadrant.Sur)),

            ["congreso eucaristico"] = (RoadType.AvenidaCarrera, new RoadNumber(68)),

            ["ciudad de lima"] = (RoadType.AvenidaCalle, new RoadNumber(19)),

            ["comuneros"] = (RoadType.AvenidaCalle, new RoadNumber(6)),
            ["los comuneros"] = (RoadType.AvenidaCalle, new RoadNumber(6)),
            ["de los comuneros"] = (RoadType.AvenidaCalle, new RoadNumber(6)),

            ["pepe sierra"] = (RoadType.AvenidaCalle, new RoadNumber(116)),

            ["esperanza"] = (RoadType.AvenidaCalle, new RoadNumber(24)),
            ["la esperanza"] = (RoadType.AvenidaCalle, new RoadNumber(24)),
        };

    public static bool TryGetRoad(
        string name, out RoadType roadType, [NotNullWhen(true)] out RoadNumber? road)
    {
        if (_avenues.TryGetValue(name, out var avenue))
        {
            (roadType, road) = avenue;
            return true;
        }

        roadType = default;
        road = null;
        return false;
    }
}
