namespace DoorstepCO.Core;

/// <summary>
/// Designation of a road within an address, e.g. 59A BIS B in "Calle 59A Bis B # 80-12".
/// </summary>
/// <param name="Number">Numeric part of the designation.</param>
/// <param name="Letter">Optional uppercase letter following the number (A in "59A").</param>
/// <param name="IsBis">Whether the designation carries the "bis" modifier.</param>
/// <param name="BisLetter">Optional uppercase letter following "bis" (B in "59A BIS B").</param>
public sealed record RoadNumber(int Number, char? Letter = null, bool IsBis = false, char? BisLetter = null);
