namespace DoorstepCO.Core.Formatting;

/// <summary>
/// DIAN-style abbreviations for each <see cref="RoadType"/>.
/// </summary>
internal static class RoadTypeAbbreviations
{
    public static string For(RoadType roadType) => roadType switch
    {
        RoadType.Calle => "CL",
        RoadType.Carrera => "CRA",
        RoadType.Diagonal => "DG",
        RoadType.Transversal => "TV",
        RoadType.Avenida => "AV",
        RoadType.AvenidaCalle => "AC",
        RoadType.AvenidaCarrera => "AK",
        _ => throw new ArgumentOutOfRangeException(nameof(roadType), roadType, "Unknown road type."),
    };
}
