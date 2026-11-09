namespace DoorstepCO.Core;

/// <summary>
/// Kind of problem detected while validating an address.
/// </summary>
public enum IssueCode
{
    /// <summary>The input is empty or contains only whitespace.</summary>
    Empty,

    /// <summary>The input could not be parsed as an urban address.</summary>
    Unparseable,

    /// <summary>
    /// The address mentions rural places (vereda, finca, km, ...) and does not follow the urban nomenclature.
    /// </summary>
    RuralAddress,

    /// <summary>
    /// Text after the address was not recognized and was ignored, e.g. "frente al parque" in
    /// "Calle 59 # 80-12 frente al parque".
    /// </summary>
    UnrecognizedText,

    /// <summary>The road, the crossing road or the plate number is 0, e.g. "Calle 59 # 80-0".</summary>
    ZeroNumber,

    /// <summary>The plate number is suspiciously high, e.g. "Calle 59 # 80-850".</summary>
    PlateNumberOutOfRange,
}
