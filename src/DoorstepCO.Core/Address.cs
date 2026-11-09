namespace DoorstepCO.Core;

/// <summary>
/// Structured components of a Colombian urban address, e.g. "Calle 59 # 80-12".
/// </summary>
/// <param name="RoadType">Type of the main road (calle, carrera, ...).</param>
/// <param name="RoadNumber">Number of the main road (59 in "Calle 59 # 80-12").</param>
/// <param name="CrossNumber">Number of the nearest crossing road (80 in "Calle 59 # 80-12").</param>
/// <param name="PlateNumber">Distance-based plate number from the crossing (12 in "Calle 59 # 80-12").</param>
public sealed record Address(RoadType RoadType, int RoadNumber, int CrossNumber, int PlateNumber);
