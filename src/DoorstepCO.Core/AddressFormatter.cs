using System.Globalization;
using DoorstepCO.Core.Formatting;

namespace DoorstepCO.Core;

/// <summary>
/// Formats parsed addresses into their standard text representation.
/// </summary>
public static class AddressFormatter
{
    private const string PlateNumberFormat = "00";

    /// <summary>
    /// Formats an address using DIAN-style abbreviations, e.g. "CL 59A BIS SUR 80 12". The plate number has at least
    /// two digits, e.g. "CL 10 4 05". The quadrant of the main road follows the road; the quadrant of the crossing
    /// road follows the plate number, e.g. "CRA 7 10 20 SUR". Complements follow, in their original order, e.g.
    /// "CRA 7 10 20 SUR TO 2 AP 302", and unrecognized remarks go last, e.g. "CL 10 5 51 FRENTE AL PARQUE".
    /// </summary>
    /// <param name="address">The address to format.</param>
    /// <returns>The standard text form of the address.</returns>
    public static string Format(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        var parts = new List<string> { RoadTypeAbbreviations.For(address.RoadType), FormatRoadNumber(address.Road) };
        AddQuadrant(parts, address.Road.Quadrant);
        parts.Add(FormatRoadNumber(address.Cross));
        parts.Add(address.PlateNumber.ToString(PlateNumberFormat, CultureInfo.InvariantCulture));
        AddQuadrant(parts, address.Cross.Quadrant);
        AddComplementsAndRemarks(parts, address.Complements, address.Remarks);
        return string.Join(' ', parts);
    }

    /// <summary>
    /// Formats complements without a road, for non-standard addresses such as "MZ 4 CA 12".
    /// </summary>
    internal static string FormatComplements(IReadOnlyList<Complement> complements, string? remarks)
    {
        var parts = new List<string>();
        AddComplementsAndRemarks(parts, complements, remarks);
        return string.Join(' ', parts);
    }

    private static void AddComplementsAndRemarks(
        List<string> parts, IReadOnlyList<Complement> complements, string? remarks)
    {
        foreach (var complement in complements)
        {
            parts.Add(ComplementAbbreviations.For(complement.Type));
            parts.Add(complement.Value);
        }

        if (remarks is not null)
        {
            parts.Add(remarks);
        }
    }

    private static string FormatRoadNumber(RoadNumber road)
    {
        if (road.Name is { } name)
        {
            return name;
        }

        var parts = new List<string>
        {
            road.Number.ToString(CultureInfo.InvariantCulture) + road.Letter + road.SecondLetter,
        };
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
