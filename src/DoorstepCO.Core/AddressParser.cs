using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DoorstepCO.Core.Normalization;
using DoorstepCO.Core.Parsing;

namespace DoorstepCO.Core;

/// <summary>
/// Parses free-text Colombian addresses into structured components.
/// </summary>
public static class AddressParser
{
    /// <summary>
    /// Tries to parse a free-text address such as "cll 59 # 80-12".
    /// </summary>
    /// <param name="input">The raw address text.</param>
    /// <param name="address">The parsed address, or <see langword="null"/> when parsing fails.</param>
    /// <returns><see langword="true"/> when the input was parsed; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string input, [NotNullWhen(true)] out Address? address)
    {
        address = null;
        var tokens = AddressTokenizer.Tokenize(input);
        var position = 0;

        if (!TryReadRoadType(tokens, ref position, out var roadType)
            || !TryReadRoadNumber(tokens, ref position, out var road))
        {
            return false;
        }

        SkipIf(tokens, ref position, PlateSeparatorKeywords.IsPlateSeparator);

        if (!TryReadRoadNumber(tokens, ref position, out var cross))
        {
            return false;
        }

        SkipIf(tokens, ref position, token => token == AddressTokenizer.Dash);

        if (!TryReadNumber(tokens, ref position, out var plateNumber) || position != tokens.Count)
        {
            return false;
        }

        address = new Address(roadType, road, cross, plateNumber);
        return true;
    }

    private static bool TryReadRoadType(IReadOnlyList<string> tokens, ref int position, out RoadType roadType)
    {
        roadType = default;
        if (position >= tokens.Count || !RoadTypeKeywords.TryGetRoadType(tokens[position], out roadType))
        {
            return false;
        }

        position++;
        return true;
    }

    private static bool TryReadRoadNumber(
        IReadOnlyList<string> tokens, ref int position, [NotNullWhen(true)] out RoadNumber? road)
    {
        road = null;
        if (!TryReadNumber(tokens, ref position, out var number))
        {
            return false;
        }

        char? letter = null;
        if (position < tokens.Count && RoadLetters.TryGetLetter(tokens[position], out var found))
        {
            letter = found;
            position++;
        }

        road = new RoadNumber(number, letter);
        return true;
    }

    private static bool TryReadNumber(IReadOnlyList<string> tokens, ref int position, out int number)
    {
        number = 0;
        if (position >= tokens.Count
            || !int.TryParse(tokens[position], NumberStyles.None, CultureInfo.InvariantCulture, out number))
        {
            return false;
        }

        position++;
        return true;
    }

    private static void SkipIf(IReadOnlyList<string> tokens, ref int position, Func<string, bool> predicate)
    {
        if (position < tokens.Count && predicate(tokens[position]))
        {
            position++;
        }
    }
}
