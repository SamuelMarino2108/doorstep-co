namespace DoorstepCO.Core;

/// <summary>
/// Designation of a road within an address, e.g. 59A in "Calle 59A # 80-12".
/// </summary>
/// <param name="Number">Numeric part of the designation.</param>
/// <param name="Letter">Optional uppercase letter following the number (A in "59A").</param>
public sealed record RoadNumber(int Number, char? Letter = null);
