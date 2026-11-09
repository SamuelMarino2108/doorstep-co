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
        _ => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };

    public static string For(IssueCode code, string detail) => $"{For(code)} \"{detail}\"";
}
