namespace DoorstepCO.Core.Validation;

/// <summary>
/// Human-readable description of each <see cref="IssueCode"/>.
/// </summary>
internal static class IssueMessages
{
    public static string For(IssueCode code) => code switch
    {
        IssueCode.Empty => "The address is empty.",
        IssueCode.Unparseable => "The address could not be recognized as an urban address.",
        IssueCode.RuralAddress => "The address looks rural and does not follow the standard urban nomenclature.",
        IssueCode.UnrecognizedText => "Text after the address was not recognized and was ignored.",
        IssueCode.ZeroNumber => "The road, crossing road or plate number is 0.",
        IssueCode.PlateNumberOutOfRange => $"The plate number is greater than {AddressLimits.MaxPlateNumber}.",
        IssueCode.NamedRoad => "The main road is designated by a name that could not be matched to its standard number.",
        _ => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };

    public static string For(IssueCode code, string detail) => $"{For(code)} \"{detail}\"";
}
