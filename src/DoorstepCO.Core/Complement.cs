namespace DoorstepCO.Core;

/// <summary>
/// A unit within the plate, e.g. "Apto 302" or "Torre B".
/// </summary>
/// <param name="Type">Type of the complement.</param>
/// <param name="Value">Uppercase identifier of the unit (302 in "Apto 302", 302B in "Apto 302 B").</param>
public sealed record Complement(ComplementType Type, string Value);
