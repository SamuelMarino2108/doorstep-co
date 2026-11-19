namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Names of Colombian departments and main cities (lowercase, accent-free tokens) that people often append to an
/// address, e.g. "Bogotá D.C." in "Cra 7 # 10-20, Bogotá D.C.". They are not part of the urban nomenclature.
/// </summary>
internal static class LocationKeywords
{
    private static readonly string[][] _locations = new[]
        {
            "colombia", "d c", "dc",

            "amazonas", "antioquia", "arauca", "atlantico", "bolivar", "boyaca", "caldas", "caqueta", "casanare",
            "cauca", "cesar", "choco", "cordoba", "cundinamarca", "guainia", "guaviare", "huila", "la guajira",
            "guajira", "magdalena", "meta", "narino", "norte de santander", "putumayo", "quindio", "risaralda",
            "san andres", "san andres y providencia", "santander", "sucre", "tolima", "valle del cauca", "valle",
            "vaupes", "vichada",

            "bogota", "medellin", "cali", "barranquilla", "cartagena", "cucuta", "bucaramanga", "pereira",
            "santa marta", "ibague", "pasto", "manizales", "neiva", "villavicencio", "armenia", "valledupar",
            "monteria", "sincelejo", "popayan", "tunja", "riohacha", "quibdo", "florencia", "yopal", "soacha",
            "bello", "itagui", "envigado", "sabaneta", "la estrella", "copacabana", "rionegro", "apartado",
            "soledad", "malambo", "chia", "zipaquira", "facatativa", "mosquera", "funza", "madrid", "cajica",
            "fusagasuga", "girardot", "dosquebradas", "palmira", "buenaventura", "tulua", "cartago", "jamundi",
            "floridablanca", "giron", "piedecuesta", "barrancabermeja", "duitama", "sogamoso", "maicao",
        }.Select(location => location.Split(' ')).ToArray();

    /// <summary>
    /// Whether the tokens from <paramref name="start"/> to the end are one or more location names and nothing else.
    /// </summary>
    public static bool IsLocationSuffix(IReadOnlyList<string> tokens, int start) =>
        start < tokens.Count && ConsumesToEnd(tokens, start);

    private static bool ConsumesToEnd(IReadOnlyList<string> tokens, int position) =>
        position == tokens.Count
        || _locations.Any(location => Matches(tokens, position, location)
            && ConsumesToEnd(tokens, position + location.Length));

    private static bool Matches(IReadOnlyList<string> tokens, int position, string[] location) =>
        position + location.Length <= tokens.Count
        && location.Select((word, i) => tokens[position + i] == word).All(match => match);
}
