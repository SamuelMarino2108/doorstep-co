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
    /// Text after the address was not recognized and was kept as remarks, e.g. "frente al parque" in
    /// "Calle 59 # 80-12 frente al parque".
    /// </summary>
    UnrecognizedText,

    /// <summary>The road, the crossing road or the plate number is 0, e.g. "Calle 59 # 80-0".</summary>
    ZeroNumber,

    /// <summary>The plate number is suspiciously high, e.g. "Calle 59 # 80-850".</summary>
    PlateNumberOutOfRange,

    /// <summary>
    /// The main road is designated by a name that could not be matched to its standard number, e.g.
    /// "Av. El Poblado # 30-28".
    /// </summary>
    NamedRoad,

    /// <summary>
    /// A complement keyword has no value or its value was not recognized, e.g. "apto" in
    /// "Calle 59 # 80-12 apto". The text is kept as remarks.
    /// </summary>
    IncompleteComplement,

    /// <summary>
    /// The road type was misspelled and corrected, e.g. "carrerra" read as carrera in "Carrerra 7 # 10-20".
    /// </summary>
    CorrectedTypo,
}
