namespace DoorstepCO.Core.Formatting;

/// <summary>
/// Standard output form of road number modifiers.
/// </summary>
internal static class ModifierAbbreviations
{
    public const string Bis = "BIS";

    public static string For(Quadrant quadrant) => quadrant switch
    {
        Quadrant.Sur => "SUR",
        Quadrant.Este => "ESTE",
        _ => throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, "Unknown quadrant."),
    };
}
