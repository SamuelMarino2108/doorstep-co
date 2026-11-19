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
        if (Parse(input) is { UnrecognizedText.Length: 0 } outcome)
        {
            address = outcome.Address;
            return true;
        }

        address = null;
        return false;
    }

    /// <summary>
    /// Like <see cref="TryParse(string, out Address?)"/>, but tolerates unrecognized text after the address: it
    /// returns the recognized part and the normalized tokens that were left over, or <see langword="null"/> when
    /// the input does not start with an address.
    /// </summary>
    internal static ParseOutcome? Parse(string input)
    {
        var tokens = AddressTokenizer.Tokenize(input);
        var position = 0;

        var quadrants = new List<(Quadrant Quadrant, bool WrittenOnRoad)>();

        if (!TryReadRoadType(tokens, ref position, out var roadType)
            || !TryReadRoad(tokens, ref position, ref roadType, quadrants, out var road))
        {
            return null;
        }

        ReadQuadrants(tokens, ref position, writtenOnRoad: true, quadrants);
        SkipIf(tokens, ref position, PlateSeparatorKeywords.IsPlateSeparator);

        if (!TryReadRoadNumber(tokens, ref position, out var cross))
        {
            return null;
        }

        ReadQuadrants(tokens, ref position, writtenOnRoad: false, quadrants);
        SkipIf(tokens, ref position, token => token == AddressTokenizer.Dash);

        if (!TryReadNumber(tokens, ref position, out var plateNumber))
        {
            return null;
        }

        ReadQuadrants(tokens, ref position, writtenOnRoad: false, quadrants);

        var stop = ComplementReader.ReadValid(tokens, position, out var complements);

        if (!QuadrantAssignment.TryAssign(roadType, quadrants, out var roadQuadrant, out var crossQuadrant))
        {
            return null;
        }

        var unrecognizedText = string.Join(' ', tokens.Skip(stop).Take(LocationSuffixStart(tokens, stop) - stop));
        var address = new Address(
            roadType,
            road with { Quadrant = roadQuadrant },
            cross with { Quadrant = crossQuadrant },
            plateNumber)
        {
            Complements = complements,
        };
        return new ParseOutcome(address, unrecognizedText);
    }

    private static int LocationSuffixStart(IReadOnlyList<string> tokens, int start)
    {
        for (var position = start; position < tokens.Count; position++)
        {
            if (LocationKeywords.IsLocationSuffix(tokens, position))
            {
                return position;
            }
        }

        return tokens.Count;
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
        if (TryReadRoadNumber(tokens, ref position, out road, isMainRoad: true))
        {
            return true;
        }

        if (!RoadTypeKeywords.CanBeNamed(roadType))
        {
            return TryReadOrdinalRoad(tokens, ref position, out road);
        }

        var name = ReadName(tokens, ref position);
        if (name.Length == 0)
        {
            return false;
        }

        if (roadType != RoadType.Avenida || !NamedAvenues.TryGetRoad(name, out var namedType, out var namedRoad))
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

    private static bool TryReadOrdinalRoad(
        IReadOnlyList<string> tokens, ref int position, [NotNullWhen(true)] out RoadNumber? road)
    {
        road = null;
        if (position >= tokens.Count || !OrdinalKeywords.TryGetNumber(tokens[position], out var number))
        {
            return false;
        }

        position++;
        road = ReadModifiers(tokens, ref position, number, isMainRoad: true);
        return true;
    }

    /// <summary>
    /// Reads the name of a road designated by name. A quadrant word ends the name, except as its first word, where
    /// it is the name itself ("autopista sur", "autopista norte").
    /// </summary>
    private static string ReadName(IReadOnlyList<string> tokens, ref int position)
    {
        var words = new List<string>();
        while (position < tokens.Count && IsNameWord(tokens[position], isFirst: words.Count == 0))
        {
            words.Add(tokens[position]);
            position++;
        }

        return string.Join(' ', words);
    }

    private static bool IsNameWord(string token, bool isFirst) =>
        char.IsLetter(token[0])
        && !PlateSeparatorKeywords.IsPlateSeparator(token)
        && ((isFirst && token.Length > 1) || !IsQuadrantWord(token));

    private static bool IsQuadrantWord(string token) =>
        QuadrantKeywords.TryGetQuadrant(token, out _) || ImplicitQuadrantKeywords.IsImplicitQuadrant(token);

    private static bool TryReadRoadNumber(
        IReadOnlyList<string> tokens,
        ref int position,
        [NotNullWhen(true)] out RoadNumber? road,
        bool isMainRoad = false)
    {
        road = null;
        if (!TryReadNumber(tokens, ref position, out var number))
        {
            return false;
        }

        road = ReadModifiers(tokens, ref position, number, isMainRoad);
        return true;
    }

    /// <summary>
    /// Reads what follows a road number: an ordinal suffix, letters and "bis". On the main road the letters may
    /// be written after a dash ("calle 59-a"); elsewhere a dash separates the plate number.
    /// </summary>
    private static RoadNumber ReadModifiers(
        IReadOnlyList<string> tokens, ref int position, int number, bool isMainRoad)
    {
        SkipIf(tokens, ref position, OrdinalKeywords.IsOrdinalSuffix);
        if (isMainRoad
            && position + 1 < tokens.Count
            && tokens[position] == AddressTokenizer.Dash
            && RoadLetters.TryGetLetter(tokens[position + 1], out _))
        {
            position++;
        }

        var letter = ReadLetter(tokens, ref position);
        var isBis = position < tokens.Count && BisKeywords.IsBis(tokens[position]);
        char? bisLetter = null;
        if (isBis)
        {
            position++;
            bisLetter = ReadBisLetter(tokens, ref position);
        }

        return new RoadNumber(number, letter, isBis, bisLetter);
    }

    private static void ReadQuadrants(
        IReadOnlyList<string> tokens,
        ref int position,
        bool writtenOnRoad,
        List<(Quadrant Quadrant, bool WrittenOnRoad)> quadrants)
    {
        while (position < tokens.Count)
        {
            if (QuadrantKeywords.TryGetQuadrant(tokens[position], out var quadrant))
            {
                quadrants.Add((quadrant, writtenOnRoad));
            }
            else if (!ImplicitQuadrantKeywords.IsImplicitQuadrant(tokens[position]))
            {
                return;
            }

            position++;
        }
    }

    private static string? ReadLetter(IReadOnlyList<string> tokens, ref int position)
    {
        if (position < tokens.Count && RoadLetters.TryGetLetter(tokens[position], out var letter))
        {
            position++;
            return letter;
        }

        return null;
    }

    private static char? ReadBisLetter(IReadOnlyList<string> tokens, ref int position)
    {
        if (position < tokens.Count
            && RoadLetters.TryGetLetter(tokens[position], out var letter)
            && letter.Length == 1)
        {
            position++;
            return letter[0];
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
