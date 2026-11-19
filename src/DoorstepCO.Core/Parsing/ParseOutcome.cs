namespace DoorstepCO.Core.Parsing;

/// <summary>
/// Result of parsing an address that may have unrecognized text after it.
/// </summary>
/// <param name="Address">The recognized part of the address.</param>
/// <param name="UnrecognizedText">The normalized tokens left over after the address, separated by spaces.</param>
internal sealed record ParseOutcome(Address Address, string UnrecognizedText);
