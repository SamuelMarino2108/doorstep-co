namespace DoorstepCO.Core;

/// <summary>
/// Designation of a road within an address, e.g. 59 in "Calle 59 # 80-12".
/// </summary>
/// <param name="Number">Numeric part of the designation.</param>
public sealed record RoadNumber(int Number);
