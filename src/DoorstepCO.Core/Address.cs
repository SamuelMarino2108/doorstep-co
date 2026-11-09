namespace DoorstepCO.Core;

/// <summary>
/// Structured components of a Colombian urban address, e.g. "Calle 59 # 80-12 Apto 302".
/// </summary>
/// <param name="RoadType">Type of the main road (calle, carrera, ...).</param>
/// <param name="Road">Designation of the main road (59 in "Calle 59 # 80-12").</param>
/// <param name="Cross">Designation of the nearest crossing road (80 in "Calle 59 # 80-12").</param>
/// <param name="PlateNumber">Distance-based plate number from the crossing (12 in "Calle 59 # 80-12").</param>
public sealed record Address(RoadType RoadType, RoadNumber Road, RoadNumber Cross, int PlateNumber)
{
    /// <summary>
    /// Units within the plate, in the order they were written (torre 2, apto 302, ...). Two addresses are
    /// equal only when their complements are equal and in the same order.
    /// </summary>
    public IReadOnlyList<Complement> Complements { get; init; } = [];

    /// <inheritdoc/>
    public bool Equals(Address? other) =>
        other is not null
        && RoadType == other.RoadType
        && Road == other.Road
        && Cross == other.Cross
        && PlateNumber == other.PlateNumber
        && Complements.SequenceEqual(other.Complements);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(RoadType);
        hash.Add(Road);
        hash.Add(Cross);
        hash.Add(PlateNumber);
        foreach (var complement in Complements)
        {
            hash.Add(complement);
        }

        return hash.ToHashCode();
    }
}
