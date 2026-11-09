using System.Globalization;
using DoorstepCO.Core.Formatting;

namespace DoorstepCO.Core;

/// <summary>
/// Formats parsed addresses into their standard text representation.
/// </summary>
public static class AddressFormatter
{
    /// <summary>
    /// Formats an address using DIAN-style abbreviations, e.g. "CL 59A BIS SUR 80 12". The quadrant of the
    /// main road follows the road; the quadrant of the crossing road goes last, e.g. "KR 7 10 20 SUR".
    /// </summary>
    /// <param name="address">The address to format.</param>
    /// <returns>The standard text form of the address.</returns>
    public static string Format(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        var parts = new List<string> { RoadTypeAbbreviations.For(address.RoadType), FormatRoadNumber(address.Road) };
        AddQuadrant(parts, address.Road.Quadrant);
        parts.Add(FormatRoadNumber(address.Cross));
        parts.Add(address.PlateNumber.ToString(CultureInfo.InvariantCulture));
        AddQuadrant(parts, address.Cross.Quadrant);

        return string.Join(' ', parts);
    }

    private static string FormatRoadNumber(RoadNumber road)
    {
        var parts = new List<string> { road.Number.ToString(CultureInfo.InvariantCulture) + road.Letter };
        if (road.IsBis)
        {
            parts.Add(ModifierAbbreviations.Bis);
        }

        if (road.BisLetter is { } bisLetter)
        {
            parts.Add(bisLetter.ToString());
        }

        return string.Join(' ', parts);
    }

    private static void AddQuadrant(List<string> parts, Quadrant? quadrant)
    {
        if (quadrant is { } value)
        {
            parts.Add(ModifierAbbreviations.For(value));
        }
    }
}
