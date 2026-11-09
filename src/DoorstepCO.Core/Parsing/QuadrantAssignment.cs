namespace DoorstepCO.Core.Parsing;

/// <summary>
/// Decides which road each written quadrant belongs to: "Sur" always qualifies the calle and "Este" always
/// qualifies the carrera, wherever it was written. When the road type does not tell (plain avenida), the
/// quadrant stays where it was written.
/// </summary>
internal static class QuadrantAssignment
{
    public static bool TryAssign(
        RoadType roadType,
        IEnumerable<(Quadrant Quadrant, bool WrittenOnRoad)> written,
        out Quadrant? roadQuadrant,
        out Quadrant? crossQuadrant)
    {
        roadQuadrant = null;
        crossQuadrant = null;

        foreach (var (quadrant, writtenOnRoad) in written)
        {
            if (BelongsToRoad(quadrant, roadType, writtenOnRoad))
            {
                if (roadQuadrant is not null)
                {
                    return false;
                }

                roadQuadrant = quadrant;
            }
            else
            {
                if (crossQuadrant is not null)
                {
                    return false;
                }

                crossQuadrant = quadrant;
            }
        }

        return true;
    }

    private static bool BelongsToRoad(Quadrant quadrant, RoadType roadType, bool writtenOnRoad)
    {
        var roadIsCalle = IsCalleLike(roadType);
        if (roadIsCalle is null)
        {
            return writtenOnRoad;
        }

        var quadrantQualifiesCalle = quadrant == Quadrant.Sur;
        return quadrantQualifiesCalle == roadIsCalle.Value;
    }

    private static bool? IsCalleLike(RoadType roadType) => roadType switch
    {
        RoadType.Calle or RoadType.Diagonal or RoadType.AvenidaCalle => true,
        RoadType.Carrera or RoadType.Transversal or RoadType.AvenidaCarrera => false,
        _ => null,
    };
}
