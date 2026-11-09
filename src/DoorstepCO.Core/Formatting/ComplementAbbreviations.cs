namespace DoorstepCO.Core.Formatting;

/// <summary>
/// DIAN abbreviations for each <see cref="ComplementType"/>, from the DIAN (MUISCA) nomenclature table.
/// </summary>
internal static class ComplementAbbreviations
{
    public static string For(ComplementType type) => type switch
    {
        ComplementType.Apartamento => "AP",
        ComplementType.Torre => "TO",
        ComplementType.Interior => "IN",
        ComplementType.Bloque => "BL",
        ComplementType.Casa => "CA",
        ComplementType.Manzana => "MZ",
        ComplementType.Lote => "LT",
        ComplementType.Conjunto => "CONJ",
        ComplementType.ConjuntoResidencial => "CON",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown complement type."),
    };
}
