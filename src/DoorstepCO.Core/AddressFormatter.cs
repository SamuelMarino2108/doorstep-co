using System.Globalization;
using DoorstepCO.Core.Formatting;

namespace DoorstepCO.Core;

/// <summary>
/// Formats parsed addresses into their standard text representation.
/// </summary>
public static class AddressFormatter
{
    /// <summary>
    /// Formats an address using DIAN-style abbreviations, e.g. "CL 59 80 12".
    /// </summary>
    /// <param name="address">The address to format.</param>
    /// <returns>The standard text form of the address.</returns>
    public static string Format(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        return string.Join(
            ' ',
            RoadTypeAbbreviations.For(address.RoadType),
            FormatRoadNumber(address.Road),
            FormatRoadNumber(address.Cross),
            address.PlateNumber.ToString(CultureInfo.InvariantCulture));
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
}
