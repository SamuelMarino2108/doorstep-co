namespace DoorstepCO.Core;

/// <summary>
/// A problem detected while validating an address.
/// </summary>
/// <param name="Code">Kind of problem.</param>
/// <param name="Message">Human-readable description of the problem.</param>
public sealed record AddressIssue(IssueCode Code, string Message);
