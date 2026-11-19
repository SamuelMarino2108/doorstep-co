using DoorstepCO.Core.Normalization;
using DoorstepCO.Core.Parsing;
using DoorstepCO.Core.Validation;

namespace DoorstepCO.Core;

/// <summary>
/// Validates free-text Colombian addresses: parses and normalizes them, detects issues and scores how likely they
/// are deliverable as written.
/// </summary>
public static class AddressValidator
{
    /// <summary>
    /// Validates a free-text address such as "cll 59 # 80-12 apto 302".
    /// </summary>
    /// <param name="input">The raw address text.</param>
    /// <returns>The parsed and normalized address, its confidence score and the issues detected.</returns>
    public static ValidationResult Validate(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(input))
        {
            return Result(null, [Issue(IssueCode.Empty)]);
        }

        var issues = new List<AddressIssue>();
        if (AddressTokenizer.Tokenize(input).Any(RuralKeywords.IsRural))
        {
            issues.Add(Issue(IssueCode.RuralAddress));
        }

        if (AddressParser.Parse(input) is not (var address, var unrecognizedText, var misspelledRoadType))
        {
            issues.Add(Issue(IssueCode.Unparseable));
            return Result(null, issues);
        }

        if (misspelledRoadType is not null)
        {
            issues.Add(new AddressIssue(
                IssueCode.CorrectedTypo, IssueMessages.For(IssueCode.CorrectedTypo, misspelledRoadType)));
        }

        if (unrecognizedText.Length > 0)
        {
            var code = StartsWithComplementKeyword(unrecognizedText)
                ? IssueCode.IncompleteComplement
                : IssueCode.UnrecognizedText;
            issues.Add(new AddressIssue(code, IssueMessages.For(code, unrecognizedText)));
            address = address with { Remarks = unrecognizedText.ToUpperInvariant() };
        }

        var roadIsZero = address.Road.Number == 0 && address.Road.Name is null;
        if (roadIsZero || address.Cross.Number == 0 || address.PlateNumber == 0)
        {
            issues.Add(Issue(IssueCode.ZeroNumber));
        }

        if (address.Road.Number > AddressLimits.MaxRoadNumber || address.Cross.Number > AddressLimits.MaxRoadNumber)
        {
            issues.Add(Issue(IssueCode.RoadNumberOutOfRange));
        }

        if (address.PlateNumber > AddressLimits.MaxPlateNumber)
        {
            issues.Add(Issue(IssueCode.PlateNumberOutOfRange));
        }

        if (address.Road.Name is not null)
        {
            issues.Add(Issue(IssueCode.NamedRoad));
        }

        return Result(address, issues);
    }

    private static bool StartsWithComplementKeyword(string text) =>
        ComplementKeywords.TryGetComplementType(text.Split(' ')[0], out _);

    private static AddressIssue Issue(IssueCode code) => new(code, IssueMessages.For(code));

    private static ValidationResult Result(Address? address, IReadOnlyList<AddressIssue> issues) =>
        new(
            address,
            address is null ? null : AddressFormatter.Format(address),
            IssuePenalties.ConfidenceFor(issues),
            issues);
}
