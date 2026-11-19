namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Maps normalized complement keywords to <see cref="ComplementType"/>. "ca" and "cs" are not casa: in DIAN
/// nomenclature "CS" is consultorio, and both are too ambiguous in free text.
/// </summary>
internal static class ComplementKeywords
{
    private static readonly Dictionary<string, ComplementType> _keywords = new(StringComparer.Ordinal)
    {
        ["apartamento"] = ComplementType.Apartamento,
        ["apto"] = ComplementType.Apartamento,
        ["apt"] = ComplementType.Apartamento,
        ["ap"] = ComplementType.Apartamento,

        ["torre"] = ComplementType.Torre,
        ["to"] = ComplementType.Torre,
        ["t"] = ComplementType.Torre,

        ["interior"] = ComplementType.Interior,
        ["int"] = ComplementType.Interior,
        ["in"] = ComplementType.Interior,

        ["bloque"] = ComplementType.Bloque,
        ["bloq"] = ComplementType.Bloque,
        ["blq"] = ComplementType.Bloque,
        ["bl"] = ComplementType.Bloque,

        ["casa"] = ComplementType.Casa,

        ["manzana"] = ComplementType.Manzana,
        ["manz"] = ComplementType.Manzana,
        ["mza"] = ComplementType.Manzana,
        ["mz"] = ComplementType.Manzana,

        ["lote"] = ComplementType.Lote,
        ["lt"] = ComplementType.Lote,

        ["conjunto"] = ComplementType.Conjunto,
        ["conj"] = ComplementType.Conjunto,
        ["cj"] = ComplementType.Conjunto,

        ["oficina"] = ComplementType.Oficina,
        ["ofi"] = ComplementType.Oficina,
        ["of"] = ComplementType.Oficina,

        ["local"] = ComplementType.Local,
        ["loc"] = ComplementType.Local,
        ["lc"] = ComplementType.Local,

        ["piso"] = ComplementType.Piso,
        ["pi"] = ComplementType.Piso,

        ["bodega"] = ComplementType.Bodega,
        ["bod"] = ComplementType.Bodega,
        ["bg"] = ComplementType.Bodega,

        ["consultorio"] = ComplementType.Consultorio,
        ["cons"] = ComplementType.Consultorio,

        ["etapa"] = ComplementType.Etapa,
        ["et"] = ComplementType.Etapa,

        ["sector"] = ComplementType.Sector,
        ["sec"] = ComplementType.Sector,
    };

    private static readonly HashSet<string> _residential = new(StringComparer.Ordinal)
    {
        "residencial",
        "res",
    };

    public static bool TryGetComplementType(string keyword, out ComplementType type) =>
        _keywords.TryGetValue(keyword, out type);

    /// <summary>
    /// Whether the keyword turns a preceding "conjunto" into a conjunto residencial.
    /// </summary>
    public static bool IsResidential(string keyword) => _residential.Contains(keyword);
}
