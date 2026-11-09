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

        if (!AddressParser.TryParse(input, out var address))
        {
            return Result(null, [Issue(IssueCode.Unparseable)]);
        }

        return Result(address, []);
    }

    private static AddressIssue Issue(IssueCode code) => new(code, IssueMessages.For(code));

    private static ValidationResult Result(Address? address, IReadOnlyList<AddressIssue> issues) =>
        new(
            address,
            address is null ? null : AddressFormatter.Format(address),
            IssuePenalties.ConfidenceFor(issues),
            issues);
}
