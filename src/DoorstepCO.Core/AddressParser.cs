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
        if (TryParse(input, out var parsed, out var unrecognizedText) && unrecognizedText.Length == 0)
        {
            address = parsed;
            return true;
        }

        address = null;
        return false;
    }

    /// <summary>
    /// Like <see cref="TryParse(string, out Address?)"/>, but tolerates unrecognized text after the address: it
    /// returns the recognized part and the normalized tokens that were left over, separated by spaces.
    /// </summary>
    internal static bool TryParse(
        string input, [NotNullWhen(true)] out Address? address, out string unrecognizedText)
    {
        address = null;
        unrecognizedText = string.Empty;
        var tokens = AddressTokenizer.Tokenize(input);
        var position = 0;

        var quadrants = new List<(Quadrant Quadrant, bool WrittenOnRoad)>();

        if (!TryReadRoadType(tokens, ref position, out var roadType)
            || !TryReadRoad(tokens, ref position, ref roadType, quadrants, out var road))
        {
            return false;
        }

        ReadQuadrants(tokens, ref position, writtenOnRoad: true, quadrants);
        SkipIf(tokens, ref position, PlateSeparatorKeywords.IsPlateSeparator);

        if (!TryReadRoadNumber(tokens, ref position, out var cross))
        {
            return false;
        }

        ReadQuadrants(tokens, ref position, writtenOnRoad: false, quadrants);
        SkipIf(tokens, ref position, token => token == AddressTokenizer.Dash);

        if (!TryReadNumber(tokens, ref position, out var plateNumber))
        {
            return false;
        }

        ReadQuadrants(tokens, ref position, writtenOnRoad: false, quadrants);

        var stop = ComplementReader.ReadValid(tokens, position, out var complements);

        if (!QuadrantAssignment.TryAssign(roadType, quadrants, out var roadQuadrant, out var crossQuadrant))
        {
            return false;
        }

        unrecognizedText = string.Join(' ', tokens.Skip(stop));
        address = new Address(
            roadType,
            road with { Quadrant = roadQuadrant },
            cross with { Quadrant = crossQuadrant },
            plateNumber)
        {
            Complements = complements,
        };
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
        if (position < tokens.Count
            && RoadTypeKeywords.TryGetRoadType(tokens[position], out var second)
            && RoadTypeKeywords.TryCombine(roadType, second, out var compound))
        {
            roadType = compound;
            position++;
        }

        return true;
    }

    private static bool TryReadRoad(
        IReadOnlyList<string> tokens,
        ref int position,
        ref RoadType roadType,
        List<(Quadrant Quadrant, bool WrittenOnRoad)> quadrants,
        [NotNullWhen(true)] out RoadNumber? road)
    {
        if (TryReadRoadNumber(tokens, ref position, out road))
        {
            return true;
        }

        if (roadType != RoadType.Avenida)
        {
            return false;
        }

        var name = ReadName(tokens, ref position);
        if (name.Length == 0)
        {
            return false;
        }

        if (!NamedAvenues.TryGetRoad(name, out var namedType, out var namedRoad))
        {
            road = new RoadNumber(0, Name: name.ToUpperInvariant());
            return true;
        }

        roadType = namedType;
        if (namedRoad.Quadrant is { } quadrant)
        {
            quadrants.Add((quadrant, WrittenOnRoad: true));
        }

        road = namedRoad with { Quadrant = null };
        return true;
    }

    private static string ReadName(IReadOnlyList<string> tokens, ref int position)
    {
        var words = new List<string>();
        while (position < tokens.Count && IsNameWord(tokens[position]))
        {
            words.Add(tokens[position]);
            position++;
        }

        return string.Join(' ', words);
    }

    private static bool IsNameWord(string token) =>
        char.IsLetter(token[0])
        && !PlateSeparatorKeywords.IsPlateSeparator(token)
        && !QuadrantKeywords.TryGetQuadrant(token, out _);

    private static bool TryReadRoadNumber(
        IReadOnlyList<string> tokens, ref int position, [NotNullWhen(true)] out RoadNumber? road)
    {
        road = null;
        if (!TryReadNumber(tokens, ref position, out var number))
        {
            return false;
        }

        var letter = ReadLetter(tokens, ref position);
        var isBis = position < tokens.Count && BisKeywords.IsBis(tokens[position]);
        char? bisLetter = null;
        if (isBis)
        {
            position++;
            bisLetter = ReadLetter(tokens, ref position);
        }

        road = new RoadNumber(number, letter, isBis, bisLetter);
        return true;
    }

    private static void ReadQuadrants(
        IReadOnlyList<string> tokens,
        ref int position,
        bool writtenOnRoad,
        List<(Quadrant Quadrant, bool WrittenOnRoad)> quadrants)
    {
        while (position < tokens.Count && QuadrantKeywords.TryGetQuadrant(tokens[position], out var quadrant))
        {
            quadrants.Add((quadrant, writtenOnRoad));
            position++;
        }
    }

    private static char? ReadLetter(IReadOnlyList<string> tokens, ref int position)
    {
        if (position < tokens.Count && RoadLetters.TryGetLetter(tokens[position], out var letter))
        {
            position++;
            return letter;
        }

        return null;
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
