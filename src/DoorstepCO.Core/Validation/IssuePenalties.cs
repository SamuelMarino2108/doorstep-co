namespace DoorstepCO.Core.Validation;

/// <summary>
/// How much each <see cref="IssueCode"/> lowers the confidence of an address. The confidence starts at 1.0, each
/// issue subtracts its penalty, and the result never drops below 0.0.
/// </summary>
internal static class IssuePenalties
{
    private const double FullConfidence = 1.0;
    private const int ConfidenceDecimals = 2;

    public static double For(IssueCode code) => code switch
    {
        IssueCode.Empty => 1.0,
        IssueCode.Unparseable => 1.0,
        IssueCode.UnrecognizedText => 0.3,
        _ => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };

    public static double ConfidenceFor(IEnumerable<AddressIssue> issues)
    {
        var confidence = FullConfidence - issues.Sum(issue => For(issue.Code));
        return Math.Round(Math.Max(0.0, confidence), ConfidenceDecimals);
    }
}
