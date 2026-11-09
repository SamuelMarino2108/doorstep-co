namespace DoorstepCO.Core;

/// <summary>
/// Type of an address complement that identifies a unit within the plate, e.g. the apartment in
/// "Calle 59 # 80-12 Apto 302".
/// </summary>
public enum ComplementType
{
    /// <summary>Apartamento (AP).</summary>
    Apartamento,

    /// <summary>Torre (TO).</summary>
    Torre,

    /// <summary>Interior (IN).</summary>
    Interior,

    /// <summary>Bloque (BL).</summary>
    Bloque,

    /// <summary>Casa (CA).</summary>
    Casa,

    /// <summary>Manzana (MZ).</summary>
    Manzana,

    /// <summary>Lote (LT).</summary>
    Lote,

    /// <summary>Conjunto (CONJ).</summary>
    Conjunto,

    /// <summary>Conjunto residencial (CON).</summary>
    ConjuntoResidencial,
}
