namespace DoorstepCO.Core;

/// <summary>
/// Designation of a road within an address, e.g. 59A BIS B SUR in "Calle 59A Bis B Sur # 80-12".
/// </summary>
/// <param name="Number">Numeric part of the designation.</param>
/// <param name="Letter">Optional uppercase letter following the number (A in "59A").</param>
/// <param name="IsBis">Whether the designation carries the "bis" modifier.</param>
/// <param name="BisLetter">Optional uppercase letter following "bis" (B in "59A BIS B").</param>
/// <param name="Quadrant">Optional city quadrant of the road (SUR in "Calle 26 Sur").</param>
/// <param name="Name">
/// Uppercase name of a road designated by name instead of number (EL POBLADO in "Av. El Poblado # 30-28"), or
/// <see langword="null"/> for numbered roads. When set, <paramref name="Number"/> is 0 and has no meaning.
/// </param>
public sealed record RoadNumber(
    int Number,
    char? Letter = null,
    bool IsBis = false,
    char? BisLetter = null,
    Quadrant? Quadrant = null,
    string? Name = null);
